﻿using API.SessionExtensions;
using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvitesController : Controller
{
    private readonly IInvitesService _inviteService;
    private readonly ILogger<InvitesController> _logger;
    private readonly ICampaignsService _campaignsService;
    private readonly INotificationsService _notificationsService;
    private readonly ISmsMessageSendingService _smsMessageSendingService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly IUsersService _usersService;

    public InvitesController(IInvitesService inviteService, ILogger<InvitesController> logger,
        ICampaignsService campaignsService, INotificationsService notificationsService,
        ISmsMessageSendingService smsMessageSendingService, IEmailSendingService emailSendingService,
        IUsersService usersService)
    {
        _inviteService = inviteService;
        _logger = logger;
        _campaignsService = campaignsService;
        _notificationsService = notificationsService;
        _smsMessageSendingService = smsMessageSendingService;
        _emailSendingService = emailSendingService;
        _usersService = usersService;
    }

    [Authorize]
    [HttpGet("/GetInvite/{campaignGuid:guid}")]
    public async Task<IActionResult> GetInvite(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignSettings,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized();
            }

            Guid? inviteGuid = await _inviteService.GetInvite(campaignGuid);
            if (inviteGuid == null)
            {
                return NotFound();
            }

            return Ok(new { InviteGuid = inviteGuid });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting invite");
            return StatusCode(500);
        }
    }

    [Authorize]
    [HttpPut("/UpdateInvite/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateInvite(Guid campaignGuid)
    {
        if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                new Permission()
                {
                    PermissionTarget = PermissionTargets.CampaignSettings,
                    PermissionType = PermissionTypes.Edit
                }))
        {
            return Unauthorized();
        }

        await _inviteService.CreateInvite(campaignGuid);
        return Ok();
    }

    [Authorize]
    [HttpDelete("/RevokeInvite/{campaignGuid:guid}")]
    public async Task<IActionResult> RevokeInvite(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignSettings,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }

            await _inviteService.RevokeInvite(campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while revoking invite");
            return StatusCode(500);
        }
    }

    [Authorize]
    [HttpPost("/AcceptInvite/{campaignInviteGuid:guid}")]
    public async Task<IActionResult> AcceptInvite(Guid campaignInviteGuid)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            var campaign = await _campaignsService.GetCampaignByInviteGuid(campaignInviteGuid);
            if (campaign == null)
            {
                return NotFound();
            }
            
            var userAccountAuthorizationStatus = HttpContext.Session.Get<bool>(Constants.UserAuthenticationStatus);
            if (!userAccountAuthorizationStatus)
            {
                return Unauthorized();
            }

            // Checks if the user is already part of the campaign and if not, adds them
            // Check is done by checking the list in the user's session, as that always contains the same data as the database
            // when it comes to this (assuming no one broke into the DB), and it's faster to check than the database.
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaign.CampaignGuid))
            {
                //await _inviteService.AcceptInvite(campaign.CampaignGuid, userId);
                //CampaignAuthorizationUtils.AddAuthorizationForCampaign(HttpContext, campaign.CampaignGuid);
                
                // Notify all users that should be notified that a new user joined the campaign
                var usersToNotify = await _notificationsService.GetUsersToNotify(campaign.CampaignGuid.Value);
                User? user = await _usersService.GetUserPublicInfo(userId);
                foreach (var userToNotify in usersToNotify)
                {
                    // Not awaited on purpose - these should just run in the background
                    if (userToNotify.ViaEmail)
                    {
                        _emailSendingService.SendUserJoinedEmailAsync(user.FirstNameHeb + " " + user.LastNameHeb,
                            campaign.CampaignName, userToNotify.Email);
                    }
                    if (userToNotify.ViaSms)
                    {
                        _smsMessageSendingService.SendUserJoinedSmsAsync(user.FirstNameHeb + " " + user.LastNameHeb,
                            campaign.CampaignName, userToNotify.PhoneNumber, CountryCodes.Israel);
                    }
                }
                
                return Ok(new { CampaignGuid = campaign });
            }

            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while accepting invite");
            return StatusCode(500);
        }
    }
}