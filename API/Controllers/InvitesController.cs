using API.SessionExtensions;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;
using static API.Utils.ErrorMessages;

// Disable warning CS4014 - there are some async methods that are not awaited, but this is intentional.
// The methods are called in the background, and the result is not needed.
#pragma warning disable CS4014

namespace API.Controllers;

/// <summary>
/// A controller for invite-related actions.<br/>
/// Provides a web API and service policy for <see cref="IInvitesService"/> methods, and also other functionality such as
/// message sending where needed.
/// </summary>
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

    /// <summary>
    /// A method for getting the invite for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get the invite for.</param>
    /// <returns>Unauthorized if the user does not have permission to view campaign settings, NotFound if the invite
    /// does not exist, Ok with the invite guid otherwise.</returns>
    [Authorize]
    [HttpGet("GetInvite/{campaignGuid:guid}")]
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            Guid? inviteGuid = await _inviteService.GetInvite(campaignGuid);
            if (inviteGuid == null)
            {
                return NotFound(FormatErrorMessage(RequestedValueNotFound, CustomStatusCode.ValueNotFound));
            }

            return Ok(new { InviteGuid = inviteGuid });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting invite");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Updates the invite for a campaign by generating a new one. If the invite does not exist, it is created.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to update the invite for.</param>
    /// <returns>Unauthorized if the user does not have permission to edit campaign settings,
    /// Ok otherwise. Use the get method again to get the new invite.</returns>
    [Authorize]
    [HttpPut("UpdateInvite/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateInvite(Guid campaignGuid)
    {
        if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                new Permission()
                {
                    PermissionTarget = PermissionTargets.CampaignSettings,
                    PermissionType = PermissionTypes.Edit
                }))
        {
            return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                CustomStatusCode.PermissionOrAuthorizationError));
        }

        await _inviteService.CreateInvite(campaignGuid);
        return Ok();
    }

    /// <summary>
    /// Deletes the invite for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to delete the invite for.</param>
    /// <returns>Unauthorized if the user does not have permission to edit campaign settings,
    /// Ok otherwise. Use the get method again to get the new invite.</returns>
    [Authorize]
    [HttpDelete("RevokeInvite/{campaignGuid:guid}")]
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
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

    /// <summary>
    /// A method for accepting an invite to a campaign.<br/>
    /// Upon accepting, the user is added to the campaign.
    /// </summary>
    /// <param name="campaignInviteGuid">The invite (a guid unique to the campaign's invite)</param>
    /// <returns>Not found if the invite matches no campaign, Unauthorized if the user did not verify their information,
    /// BadRequest if user is already a member of the campaign, Ok otherwise.</returns>
    [Authorize]
    [HttpPost("AcceptInvite/{campaignInviteGuid:guid}")]
    public async Task<IActionResult> AcceptInvite(Guid campaignInviteGuid)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            var campaign = await _campaignsService.GetCampaignByInviteGuid(campaignInviteGuid);
            if (campaign == null)
            {
                return NotFound(FormatErrorMessage(CampaignNotFound, CustomStatusCode.ValueNotFound));
            }

            var userAccountAuthorizationStatus = HttpContext.Session.Get<bool>(Constants.UserAuthenticationStatus);
            if (!userAccountAuthorizationStatus)
            {
                return Unauthorized(FormatErrorMessage(VerificationStatusError,
                    CustomStatusCode.VerificationStatusError));
            }

            // Checks if the user is already part of the campaign and if not, adds them
            // Check is done by checking the list in the user's session, as that always contains the same data as the database
            // when it comes to this (assuming no one broke into the DB), and it's faster to check than the database.
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaign.CampaignGuid))
            {
                // This condition shouldn't happen - if the previous check passed, the user should already not be 
                // in the campaign. But just in case, we check again, because we get it "for free" from the return value
                // and mistakes can happen.
                var res = await _inviteService.AcceptInvite(campaign.CampaignGuid, userId);
                if (res == CustomStatusCode.DuplicateKey)
                {
                    return BadRequest(FormatErrorMessage(AlreadyAMember, CustomStatusCode.DuplicateKey));
                }

                CampaignAuthorizationUtils.AddAuthorizationForCampaign(HttpContext, campaign.CampaignGuid);

                // Notify all users that should be notified that a new user joined the campaign
                var usersToNotify = await _notificationsService.GetUsersToNotify(campaign.CampaignGuid.Value);
                User? user = await _usersService.GetUserPublicInfo(userId);
                foreach (var userToNotify in usersToNotify)
                {
                    // Not awaited on purpose - these should just run in the background
                    if (userToNotify.ViaEmail && userToNotify.Email != null)
                    {
                        _emailSendingService.SendUserJoinedEmailAsync(user.FirstNameHeb + " " + user.LastNameHeb,
                            campaign.CampaignName, userToNotify.Email);
                    }

                    if (userToNotify.ViaSms && userToNotify.PhoneNumber != null)
                    {
                        _smsMessageSendingService.SendUserJoinedSmsAsync(user.FirstNameHeb + " " + user.LastNameHeb,
                            campaign.CampaignName, userToNotify.PhoneNumber, CountryCodes.Israel);
                    }
                }

                return Ok(new { CampaignGuid = campaign });
            }

            return BadRequest(FormatErrorMessage(AlreadyAMember, CustomStatusCode.DuplicateKey));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while accepting invite");
            return StatusCode(500);
        }
    }
}