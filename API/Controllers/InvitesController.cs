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
    
    public InvitesController(IInvitesService inviteService, ILogger<InvitesController> logger)
    {
        _inviteService = inviteService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("<campaignGuid>/GetInvite")]
    public async Task<IActionResult> GetInvite(Guid campaignGuid)
    {
        if (!AuthenticationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
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
    public async Task<IActionResult> UpdateInvite(Guid campaignGuid, [FromBody] Guid inviteGuid)
    {
        if (!AuthenticationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
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
        if (!AuthenticationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
        {
            return Unauthorized();
        }

        await _inviteService.RevokeInvite(campaignGuid);
        return Ok();
    }
}