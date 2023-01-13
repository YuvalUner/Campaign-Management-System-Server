using API.SessionExtensions;
using API.Utils;
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
    
    public InvitesController(IInvitesService inviteService, ILogger<InvitesController> logger, ICampaignsService campaignsService)
    {
        _inviteService = inviteService;
        _logger = logger;
        _campaignsService = campaignsService;
    }

    [Authorize]
    [HttpGet("<campaignGuid>/GetInvite")]
    public async Task<IActionResult> GetInvite(Guid campaignGuid)
    {
        if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
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
    
    [Authorize]
    [HttpPut("<campaignGuid>/UpdateInvite")]
    public async Task<IActionResult> UpdateInvite(Guid campaignGuid)
    {
        if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
        {
            return Unauthorized();
        }

        await _inviteService.CreateInvite(campaignGuid);
        return Ok();
    }
    
    [Authorize]
    [HttpDelete("<campaignGuid>/RevokeInvite")]
    public async Task<IActionResult> RevokeInvite(Guid campaignGuid)
    {
        if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
        {
            return Unauthorized();
        }

        await _inviteService.RevokeInvite(campaignGuid);
        return Ok();
    }
    
    [Authorize]
    [HttpPost("<campaignInviteGuid>/AcceptInvite")]
    public async Task<IActionResult> AcceptInvite(Guid campaignInviteGuid)
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
}