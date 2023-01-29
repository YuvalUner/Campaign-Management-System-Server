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
public class JobTypeAssignmentCapabilityController : Controller
{
    private readonly IJobTypeAssignmentCapabilityService _jobTypeAssignmentCapabilityService;
    private readonly ILogger<JobTypeAssignmentCapabilityController> _logger;
    
    public JobTypeAssignmentCapabilityController(IJobTypeAssignmentCapabilityService jobTypeAssignmentCapabilityService,
        ILogger<JobTypeAssignmentCapabilityController> logger)
    {
        _jobTypeAssignmentCapabilityService = jobTypeAssignmentCapabilityService;
        _logger = logger;
    }
    
    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddJobTypeAssignmentCapableUser(Guid campaignGuid, [FromBody] JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.JobTypes,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var result = await _jobTypeAssignmentCapabilityService.AddJobTypeAssignmentCapableUser(campaignGuid, jobTypeAssignmentCapabilityParams);
            return result switch
            {
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound,
                    CustomStatusCode.UserNotFound)),
                CustomStatusCode.JobTypeNotFound => NotFound(FormatErrorMessage(JobTypeNotFound, CustomStatusCode.JobTypeNotFound)),
                CustomStatusCode.DuplicateKey => Conflict(FormatErrorMessage(CanAlreadyAssignToJobType,
                    CustomStatusCode.DuplicateKey)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding job type assignment capable user");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("remove/{campaignGuid:guid}")]
    public async Task<IActionResult> RemoveJobTypeAssignmentCapableUser(Guid campaignGuid, [FromBody] JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.JobTypes,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            await _jobTypeAssignmentCapabilityService.RemoveJobTypeAssignmentCapableUser(campaignGuid, jobTypeAssignmentCapabilityParams);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing job type assignment capable user");
            return StatusCode(500);
        }
    }
    
    [HttpGet("get/{campaignGuid:guid}/{jobTypeName}")]
    public async Task<IActionResult> GetJobTypeAssignmentCapableUsers(Guid campaignGuid, string jobTypeName)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.JobTypes,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var result = await _jobTypeAssignmentCapabilityService.GetJobTypeAssignmentCapableUsers(campaignGuid, jobTypeName);
            var resultWithoutIds = result.Select(x => new
            {
                x.DisplayNameEng,
                x.FirstNameHeb,
                x.LastNameHeb,
                x.Email,
                x.PhoneNumber,
                x.ProfilePicUrl
            });
            return Ok(resultWithoutIds);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting job type assignment capable users");
            return StatusCode(500);
        }
    }
}