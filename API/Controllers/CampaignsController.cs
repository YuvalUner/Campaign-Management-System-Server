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
    private readonly IPermissionsService _permissionsService;
    private readonly IRolesService _rolesService;
    

    public CampaignsController(ILogger<CampaignsController> logger, ICampaignsService campaignService,
        IPermissionsService permissionsService, IRolesService rolesService)
    {
        _logger = logger;
        _campaignService = campaignService;
        _permissionsService = permissionsService;
        _rolesService = rolesService;
    }

    /// <summary>
    /// An action the client is expected to perform each time they load a campaign page.
    /// This sets the current active campaign for the user, and fetches data about the user's role in it.
    /// </summary>
    /// <param name="campaignGuid"></param>
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
            
            // Set the user's permissions and role in the active campaign in session
            await PermissionUtils.SetPermissions(_permissionsService, HttpContext);
            await RoleUtils.SetRole(_rolesService, HttpContext);

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

    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateCampaign(Guid campaignGuid, [FromBody] Campaign campaign)
    {
        try
        {
            // Check if the user has access to this campaign to begin with.
            // Not checking authentication status since if they have access then they must be authenticated.
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignSettings,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }

            await _campaignService.ModifyCampaign(campaign);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating campaign");
            return StatusCode(500);
        }
    }
    
    [HttpGet("getUsers/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignUsers(Guid campaignGuid)
    {
        try
        {
            // Check if the user has access to this campaign to begin with.
            // Not checking authentication status since if they have access then they must be authenticated.
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignUsersList,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized();
            }

            var users = await _campaignService.GetUsersInCampaign(campaignGuid);
            var usersPartialInfo = users.Select(u =>
                new
                {
                    u.Email,
                    u.FirstNameEng,
                    u.LastNameEng,
                    u.FirstNameHeb,
                    u.LastNameHeb,
                });
            return Ok(usersPartialInfo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting campaign users");
            return StatusCode(500);
        }
    }
}