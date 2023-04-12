using API.SessionExtensions;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for all campaign-related actions.
/// Mostly used to expose a web API and service policy for <see cref="ICampaignsService"/>, and makes heavy usage of the model
/// <see cref="Campaign"/>.<br/>
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// This sets the current active campaign for the user, and fetches data about the user's role and permissions in it.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("enter/{campaignGuid:guid}")]
    public async Task<IActionResult> EnterCampaign(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(AuthorizationError,
                    CustomStatusCode.AuthorizationError));
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

    /// <summary>
    /// Creates a new campaign in the database, with the user as the campaign owner.
    /// </summary>
    /// <param name="campaign">A populated <see cref="Campaign"/> object, with at-least the CampaignName,
    /// IsMunicipal, IsSubCampaign, CityName fields filled in.</param>
    /// <returns>Ok with the Guid of the newly created campaign on success, Unauthorized or BadRequest
    /// with an error message otherwise.</returns>
    [Authorize]
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
                return Unauthorized(FormatErrorMessage(VerificationStatusError,
                    CustomStatusCode.VerificationStatusError));
            }

            // Verify that the user submitted all the necessary data
            if (string.IsNullOrEmpty(campaign.CampaignName)
                || campaign.IsMunicipal == null
                || campaign.IsMunicipal == true && string.IsNullOrEmpty(campaign.CityName))
            {
                return BadRequest(
                    FormatErrorMessage(CampaignNameOrCityNameRequired, CustomStatusCode.ValueCanNotBeNull));
            }

            // If the campaign is not municipal, it is a national campaign, and the city name should be set to "ארצי".
            // This is obviously not a real city, but it is valid for the database and avoids a foreign key violation.
            if (campaign.IsMunicipal == false)
            {
                campaign.CityName = "ארצי";
            }


            campaign.CampaignId = await _campaignService.AddCampaign(campaign, userId);
            if (campaign.CampaignId == -1)
            {
                return BadRequest(FormatErrorMessage(CityNotFound, CustomStatusCode.CityNotFound));
            }

            _logger.LogInformation("Created campaign called {CampaignName} for user {SenderId}", campaign.CampaignName,
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

    /// <summary>
    /// Updates a campaign in the database.
    /// </summary>
    /// <param name="campaignGuid">The guid of the campaign to update.</param>
    /// <param name="campaign">A <see cref="Campaign"/> object, with the fields to be updated set to not null.
    /// Specifically, the fields that can be updated are CampaignDescription and CampaignLogoUrl.</param>
    /// <returns></returns>
    [Authorize]
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
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

    /// <summary>
    /// Gets the list of users in a campaign.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <returns>Unauthorized if the user does not have permission to get this list in the given campaign, Ok with the
    /// list otherwise.</returns>
    [Authorize]
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var users = await _campaignService.GetUsersInCampaign(campaignGuid);

            // Return only the info that is ok to be displayed publicly (unlike id numbers and user ids).
            var usersPartialInfo = users.Select(u =>
                new
                {
                    u.Email,
                    u.FirstNameEng,
                    u.LastNameEng,
                    u.FirstNameHeb,
                    u.LastNameHeb,
                    u.RoleName
                });
            return Ok(usersPartialInfo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting campaign users");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets the basic info of a campaign - name, city, description, logo, etc - all of the info that is ok to be
    /// displayed publicly.<br/>
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign to get the info for</param>
    /// <returns>NotFound if campaign was not found, else Ok with a JSON object containing all of the campaign's public
    /// info.</returns>
    [HttpGet("get-basic-info/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignBasicInfo(Guid campaignGuid)
    {
        try
        {
            var campaign = await _campaignService.GetCampaignBasicInfo(campaignGuid);

            if (campaign == null)
            {
                return NotFound(FormatErrorMessage(CampaignNotFound, CustomStatusCode.CampaignNotFound));
            }

            return Ok(new
            {
                campaign.CampaignName,
                campaign.CityName,
                campaign.IsMunicipal,
                campaign.CampaignDescription,
                campaign.CampaignGuid,
                campaign.CampaignCreationDate,
                campaign.CampaignLogoUrl
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting campaign basic info");
            return StatusCode(500);
        }
    }
    
    /// <summary>
    /// Like <see cref="GetCampaignBasicInfo"/>, but gets the basic info of a campaign by its invite guid.
    /// </summary>
    /// <param name="inviteGuid"></param>
    /// <returns></returns>
    [HttpGet("info-by-invite-guid/{inviteGuid:guid}")]
    public async Task<IActionResult> GetCampaignInfoByInviteGuid(Guid inviteGuid)
    {
        try
        {
            var campaign = await _campaignService.GetCampaignBasicInfoByInviteGuid(inviteGuid);

            if (campaign == null)
            {
                return NotFound(FormatErrorMessage(CampaignNotFound, CustomStatusCode.CampaignNotFound));
            }

            return Ok(new
            {
                campaign.CampaignName,
                campaign.CityName,
                campaign.IsMunicipal,
                campaign.CampaignDescription,
                campaign.CampaignGuid,
                campaign.CampaignCreationDate,
                campaign.CampaignLogoUrl
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting campaign basic info");
            return StatusCode(500);
        }
    }

    [Authorize]
    [HttpGet("get-campaign-admins/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignAdmins(Guid campaignGuid)
    {
        try
        {
            var campaignAdmins = await _campaignService.GetCampaignAdmins(campaignGuid);
            return Ok(campaignAdmins);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting campaign admins");
            return StatusCode(500, "Error getting campaign admins");
        }
    }
}