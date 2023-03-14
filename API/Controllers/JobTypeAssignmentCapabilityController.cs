using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for handling job type assignment capabilities.
/// Provides a web API and service policy for <see cref="IJobTypeAssignmentCapabilityService"/>.
/// </summary>
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
    
    /// <summary>
    /// Adds a user to a job type's assignment capable users.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <param name="jobTypeAssignmentCapabilityParams">An instance of <see cref="JobTypeAssignmentCapabilityParams"/>
    /// with all values filled in.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit job types in the campaign,
    /// NotFound if the user or job type from the parameters was not found, Conflict if the user can already
    /// assign to the job type, Ok otherwise.</returns>
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
    
    /// <summary>
    /// Removes a user from a job type's assignment capable users.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <param name="jobTypeAssignmentCapabilityParams">An instance of <see cref="JobTypeAssignmentCapabilityParams"/>
    /// with all values filled in.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit job types in the campaign,
    /// Ok otherwise.</returns>
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
    
    /// <summary>
    /// Gets all users that can assign to a job type.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <param name="jobTypeName">Name of the requested job type.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to view job types in the campaign,
    /// Ok with a list with the same fields as <see cref="UserPublicInfo"/> (this was made before that model was created)
    /// otherwise.</returns>
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