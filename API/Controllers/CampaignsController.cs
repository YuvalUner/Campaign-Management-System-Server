using API.SessionExtensions;
using API.Utils;
using DAL.Models;
using DAL.Services.Implementations;
using DAL.Services.Interfaces;
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

    /// <summary>
    /// An action the client is expected to perform each time they load a campaign page.
    /// This sets the current active campaign for the user, and fetches data about the user's role in it.
    /// </summary>
    /// <param name="campaign"></param>
    /// <returns></returns>
    [HttpPost("/enter/{campaignGuid:guid}")]
    public async Task<IActionResult> EnterCampaign(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            CampaignAuthorizationUtils.EnterCampaign(HttpContext, campaignGuid);
            // TODO: Get user's role in campaign and set it in the session.
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error entering campaign");
            return StatusCode(500);
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign([FromBody] Campaign campaign)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Unauthenticated users (did not verify their identity) should not be able to create campaigns
            var authenticationStatus = HttpContext.Session.Get<bool>(Constants.UserAuthenticationStatus);
            if (!authenticationStatus)
            {
                return Unauthorized();
            }

            // Verify that the user submitted all the necessary data
            if (string.IsNullOrEmpty(campaign.CampaignName)
                || campaign.IsMunicipal == null
                || campaign.IsMunicipal == true && string.IsNullOrEmpty(campaign.CityName))
            {
                return BadRequest();
            }

            if (campaign.IsMunicipal == false)
            {
                campaign.CityName = "ארצי";
            }


            campaign.CampaignId = await _campaignService.AddCampaign(campaign, userId);
            if (campaign.CampaignId == -1)
            {
                return BadRequest("Error with city name");
            }

            _logger.LogInformation("Created campaign called {CampaignName} for user {UserId}", campaign.CampaignName,
                userId);
            // After creating the new campaign, add the newly created campaign to the list of campaigns the user can access.
            Guid? campaignGuid = await _campaignService.GetCampaignGuid(campaign.CampaignId);
            CampaignAuthorizationUtils.AddAuthorizationForCampaign(HttpContext, campaignGuid);
            return Ok(new { newCampaignGuid = campaignGuid });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating campaign");
            return StatusCode(500);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCampaign([FromBody] Campaign campaign)
    {
        try
        {
            // Check if the user has access to this campaign to begin with.
            // Not checking authentication status since if they have access then they must be authenticated.
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaign.CampaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaign.CampaignGuid))
            {
                return Unauthorized();
            }

            //***********************************************************************
            //TODO: Check that the user has permission to update this campaign.
            //***********************************************************************

            await _campaignService.ModifyCampaign(campaign);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating campaign");
            return StatusCode(500);
        }
    }
}