using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobPreferencesController : Controller
{
    private readonly IJobPreferencesService _jobPreferencesService;
    private readonly ILogger<JobPreferencesController> _logger;
    
    public JobPreferencesController(IJobPreferencesService jobPreferencesService, 
        ILogger<JobPreferencesController> logger)
    {
        _jobPreferencesService = jobPreferencesService;
        _logger = logger;
    }
    
    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddUserPreferences(Guid campaignGuid, [FromBody] UserJobPreference userJobPreference)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(userJobPreference.UserPreferencesText))
            {
                return BadRequest(FormatErrorMessage(PreferencesNullOrEmpty, CustomStatusCode.ValueNullOrEmpty));
            }
            
            int? userId = HttpContext.Session.GetInt32(Constants.UserId);
            await _jobPreferencesService.AddUserPreferences(userId, campaignGuid, userJobPreference.UserPreferencesText);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding user preferences");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("delete/{campaignGuid:guid}")]
    public async Task<IActionResult> DeleteUserPreferences(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }
            int? userId = HttpContext.Session.GetInt32(Constants.UserId);
            await _jobPreferencesService.DeleteUserPreferences(userId, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting user preferences");
            return StatusCode(500);
        }
    }
    
    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateUserPreferences(Guid campaignGuid, [FromBody] UserJobPreference userJobPreference)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }
            
            if (string.IsNullOrWhiteSpace(userJobPreference.UserPreferencesText))
            {
                return BadRequest(FormatErrorMessage(PreferencesNullOrEmpty, CustomStatusCode.ValueNullOrEmpty));
            }
            
            int? userId = HttpContext.Session.GetInt32(Constants.UserId);
            await _jobPreferencesService.UpdateUserPreferences(userId, campaignGuid, userJobPreference.UserPreferencesText);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating user preferences");
            return StatusCode(500);
        }
    }
    
    [HttpGet("get/{campaignGuid:guid}")]
    public async Task<IActionResult> GetUserPreferences(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }
            int? userId = HttpContext.Session.GetInt32(Constants.UserId);
            UserJobPreference? userJobPreference = await _jobPreferencesService.GetUserPreferences(userId, campaignGuid);
            if (userJobPreference == null)
            {
                // Avoid null reference exception in client side.
                // It is also possible to return 404, but it is not necessary.
                // Doing it this way is more convenient for the client.
                userJobPreference = new UserJobPreference()
                {
                    UserPreferencesText = string.Empty
                };
            }
            return Ok(userJobPreference);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting user preferences");
            return StatusCode(500);
        }
    }
}