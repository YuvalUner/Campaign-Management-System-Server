using API.SessionExtensions;
using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvitesController : Controller
{
    private readonly IInvitesService _inviteService;
    private readonly ILogger<InvitesController> _logger;
    private readonly ICampaignsService _campaignsService;

    public InvitesController(IInvitesService inviteService, ILogger<InvitesController> logger,
        ICampaignsService campaignsService)
    {
        _inviteService = inviteService;
        _logger = logger;
        _campaignsService = campaignsService;
    }

    [Authorize]
    [HttpGet("/GetInvite/{campaignGuid:guid}")]
    public async Task<IActionResult> GetInvite(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }
            var requiredPermission = new Permission()
            {
                PermissionType = PermissionTypes.View,
                PermissionTarget = PermissionTargets.CampaignSettings
            };
            if (!PermissionUtils.HasPermission(HttpContext, requiredPermission))
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
        if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
        {
            return Unauthorized();
        }
        var requiredPermission = new Permission()
        {
            PermissionType = PermissionTypes.Edit,
            PermissionTarget = PermissionTargets.CampaignSettings
        };
        if (!PermissionUtils.HasPermission(HttpContext, requiredPermission))
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
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }
            var requiredPermission = new Permission()
            {
                PermissionType = PermissionTypes.Edit,
                PermissionTarget = PermissionTargets.CampaignSettings
            };
            if (!PermissionUtils.HasPermission(HttpContext, requiredPermission))
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
            var campaignGuid = await _campaignsService.GetCampaignGuidByInviteGuid(campaignInviteGuid);
            if (campaignGuid == null)
            {
                return NotFound();
            }

            // Checks if the user is already part of the campaign and if not, adds them
            // Check is done by checking the list in the user's session, as that always contains the same data as the database
            // when it comes to this (assuming no one broke into the DB), and it's faster to check than the database.
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                await _inviteService.AcceptInvite(campaignGuid, userId);
                CampaignAuthorizationUtils.AddAuthorizationForCampaign(HttpContext, campaignGuid);
                return Ok(new { CampaignGuid = campaignGuid });
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