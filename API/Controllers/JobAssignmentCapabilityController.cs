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
public class JobAssignmentCapabilityController : Controller
{
    private readonly IJobAssignmentCapabilityService _jobAssignmentCapabilityService;
    private readonly ILogger<JobAssignmentCapabilityController> _logger;

    public JobAssignmentCapabilityController(IJobAssignmentCapabilityService jobAssignmentCapabilityService,
        ILogger<JobAssignmentCapabilityController> logger)
    {
        _jobAssignmentCapabilityService = jobAssignmentCapabilityService;
        _logger = logger;
    }
    
    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddJobAssignmentCapableUser(Guid campaignGuid, [FromBody] JobAssignmentCapabilityParams jobAssignmentCapabilityParams)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Jobs,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var result = await _jobAssignmentCapabilityService.AddJobAssignmentCapableUser(campaignGuid, jobAssignmentCapabilityParams);
            return result switch
            {
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound,
                    CustomStatusCode.UserNotFound)),
                CustomStatusCode.JobNotFound => NotFound(FormatErrorMessage(JobNotFound, CustomStatusCode.JobNotFound)),
                CustomStatusCode.DuplicateKey => Conflict(FormatErrorMessage(CanAlreadyAssignToJob,
                    CustomStatusCode.DuplicateKey)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding job assignment capable user");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("remove/{campaignGuid:guid}")]
    public async Task<IActionResult> RemoveJobAssignmentCapableUser(Guid campaignGuid, [FromBody] JobAssignmentCapabilityParams jobAssignmentCapabilityParams)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Jobs,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            await _jobAssignmentCapabilityService.RemoveJobAssignmentCapableUser(campaignGuid, jobAssignmentCapabilityParams);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing job assignment capable user");
            return StatusCode(500);
        }
    }
    
    [HttpGet("get/{campaignGuid:guid}/{jobGuid:guid}/{viaJobType:bool?}")]
    public async Task<IActionResult> GetJobAssignmentCapableUsers(Guid campaignGuid, Guid jobGuid, bool? viaJobType)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Jobs,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var result = await _jobAssignmentCapabilityService.GetJobAssignmentCapableUsers(campaignGuid, jobGuid, viaJobType);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting job assignment capable users");
            return StatusCode(500);
        }
    }

}