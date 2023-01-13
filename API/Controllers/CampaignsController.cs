using API.SessionExtensions;
using DAL.Models;
using DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignsController : Controller
{
    private readonly ILogger<CampaignsController> _logger;
    private readonly ICampaignsService _campaignService;
    
    public CampaignsController(ILogger<CampaignsController> logger, ICampaignsService campaignService)
    {
        _logger = logger;
        _campaignService = campaignService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign([FromBody] Campaign campaign)
    {
        var userId = HttpContext.Session.GetInt32(Constants.UserId);
        if (userId == null)
        {
            return Unauthorized();
        }

        var authenticationStatus = HttpContext.Session.Get<bool>(Constants.UserAuthenticationStatus);
        if (!authenticationStatus)
        {
            return Unauthorized();
        }
        
        // Verify that the user submitted all the necessary data
        if (string.IsNullOrEmpty(campaign.CampaignName) || campaign.IsMunicipal == null)
        {
            return BadRequest();
        }
        
        
        campaign.CampaignId = await _campaignService.AddCampaign(campaign, userId);
        _logger.LogInformation("Created campaign called {CampaignName} for user {UserId}", campaign.CampaignName, userId);
        // After creating the new campaign, add the newly created campaign to the list of campaigns the user can access.
        Guid? campaignGuid = await _campaignService.GetCampaignGuid(campaign.CampaignId);
        var allowedCampaigns = HttpContext.Session.Get<List<Guid?>?>(Constants.AllowedCampaigns);
        if (allowedCampaigns == null)
        {
            allowedCampaigns = new List<Guid?>();
        }
        allowedCampaigns.Add(campaignGuid);
        HttpContext.Session.Set(Constants.AllowedCampaigns, allowedCampaigns);
        return Ok();
    }
    
    [HttpPut("update")]
    public async Task<IActionResult> UpdateCampaign([FromBody] Campaign campaign)
    {
        // Make sure the user is a legitimate user.
        var userId = HttpContext.Session.GetInt32(Constants.UserId);
        if (userId == null)
        {
            return Unauthorized();
        }

        var authenticationStatus = HttpContext.Session.Get<bool>(Constants.UserAuthenticationStatus);
        if (!authenticationStatus)
        {
            return Unauthorized();
        }

        // Check if the user has access to this campaign to begin with.
        var userAllowedCampaigns = HttpContext.Session.Get<List<Guid?>?>(Constants.AllowedCampaigns);
        if (userAllowedCampaigns == null || !userAllowedCampaigns.Contains(campaign.CampaignGuid))
        {
            return Unauthorized();
        }
        
        //***********************************************************************
        //TODO: Check that the user has permission to update this campaign.
        //***********************************************************************
        
        await _campaignService.ModifyCampaign(campaign);
        return Ok();
    }
}