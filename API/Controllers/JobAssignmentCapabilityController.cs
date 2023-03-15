using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for job assignment capability-related actions.<br/>
/// Provides a web API and service policy for <see cref="IJobAssignmentCapabilityService"/> methods.
/// </summary>
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

    /// <summary>
    /// A method for adding a user to the list of users that can assign to a specific job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign this relates to.</param>
    /// <param name="jobAssignmentCapabilityParams">An instance of <see cref="JobAssignmentCapabilityParams"/> with all properties
    /// not null, in the request body.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit jobs in the campaign,
    /// NotFound if user to add or the job do not exist, Conflict if the user can already assign to this job,
    /// Ok otherwise.</returns>
    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddJobAssignmentCapableUser(Guid campaignGuid,
        [FromBody] JobAssignmentCapabilityParams jobAssignmentCapabilityParams)
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

            var result =
                await _jobAssignmentCapabilityService.AddJobAssignmentCapableUser(campaignGuid,
                    jobAssignmentCapabilityParams);
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

    /// <summary>
    /// A method for removing a user from the list of users that can assign to a specific job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign this relates to.</param>
    /// <param name="jobAssignmentCapabilityParams">An instance of <see cref="JobAssignmentCapabilityParams"/> with all properties
    /// not null, in the request body.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit jobs in the campaign,
    /// Ok otherwise.</returns>
    [HttpDelete("remove/{campaignGuid:guid}")]
    public async Task<IActionResult> RemoveJobAssignmentCapableUser(Guid campaignGuid,
        [FromBody] JobAssignmentCapabilityParams jobAssignmentCapabilityParams)
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

            await _jobAssignmentCapabilityService.RemoveJobAssignmentCapableUser(campaignGuid,
                jobAssignmentCapabilityParams);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing job assignment capable user");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets a list of users that can assign to a specific job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign this relates to.</param>
    /// <param name="jobGuid">Guid of the specific job.</param>
    /// <param name="viaJobType">Whether to include users who can assign to that job because they can assign in general
    /// to anyone within the job's job type or not.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to view jobs in the campaign,
    /// Ok with a list that has the same fields as <see cref="UserPublicInfo"/> (this was made before that model was made).</returns>
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

            var result =
                await _jobAssignmentCapabilityService.GetJobAssignmentCapableUsers(campaignGuid, jobGuid, viaJobType);
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
            _logger.LogError(e, "Error getting job assignment capable users");
            return StatusCode(500);
        }
    }
}