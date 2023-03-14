using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for handling job types.
/// Provides a web API and service policy for <see cref="IJobTypesService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class JobTypesController : Controller
{
    private readonly IJobTypesService _jobTypesService;
    private readonly ILogger<JobTypesController> _logger;

    public JobTypesController(IJobTypesService jobTypesService, ILogger<JobTypesController> logger)
    {
        _jobTypesService = jobTypesService;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new job type to a campaign.
    /// </summary>
    /// <param name="jobType">An instance of <see cref="JobType"/> with all the required parameters filled in.</param>
    /// <param name="campaignGuid">Guid of the campaign to add to.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit job types in the campaign,
    /// BadRequest if the job type name is a built in one, is empty, already exists for the campaign, or the campaign
    /// already has too many job types, Ok otherwise.</returns>
    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddJobType([FromBody] JobType jobType, Guid campaignGuid)
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
                return Unauthorized(FormatErrorMessage(AuthorizationError,
                    CustomStatusCode.AuthorizationError));
            }

            if (string.IsNullOrWhiteSpace(jobType.JobTypeName))
            {
                return BadRequest(FormatErrorMessage(JobTypeRequired, CustomStatusCode.ValueCanNotBeNull));
            }

            if (BuiltInJobTypes.IsBuiltIn(jobType.JobTypeName))
            {
                return BadRequest(FormatErrorMessage(NameMustNotBeBuiltIn, CustomStatusCode.NameCanNotBeBuiltIn));
            }
            
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var res = await _jobTypesService.AddJobType(jobType, campaignGuid, userId);
            return res switch
            {
                CustomStatusCode.CannotInsertDuplicateUniqueIndex => BadRequest(FormatErrorMessage(JobTypeAlreadyExists, res)),
                CustomStatusCode.TooManyEntries => BadRequest(FormatErrorMessage(TooManyJobTypes, res)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding job type");
            return StatusCode(500, "Error while adding job type");
        }
    }

    /// <summary>
    /// Deletes an existing job type from a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="jobTypeName">Name of the job type to delete.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit job types in the campaign,
    /// BadRequest if the name is of a built in job type, Ok otherwise.</returns>
    [HttpDelete("delete/{campaignGuid:guid}/{jobTypeName}")]
    public async Task<IActionResult> DeleteJobType(Guid campaignGuid, string jobTypeName)
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
                return Unauthorized(FormatErrorMessage(AuthorizationError,
                    CustomStatusCode.AuthorizationError));
            }
            
            if (BuiltInJobTypes.IsBuiltIn(jobTypeName))
            {
                return BadRequest(FormatErrorMessage(NameMustNotBeBuiltIn, CustomStatusCode.NameCanNotBeBuiltIn));
            }
            
            await _jobTypesService.DeleteJobType(jobTypeName, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting job type");
            return StatusCode(500, "Error while deleting job type");
        }
    }
    
    /// <summary>
    /// Gets a list of all job types for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to view job types in the campaign,
    /// Ok with a list of <see cref="JobType"/> otherwise.</returns>
    [HttpGet("get/{campaignGuid:guid}")]
    public async Task<IActionResult> GetJobTypes(Guid campaignGuid)
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
                return Unauthorized(FormatErrorMessage(AuthorizationError,
                    CustomStatusCode.AuthorizationError));
            }

            var jobTypes = await _jobTypesService.GetJobTypes(campaignGuid);
            return Ok(jobTypes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting job types");
            return StatusCode(500, "Error while getting job types");
        }
    }
    
    /// <summary>
    /// Updates an existing job type's name or description.
    /// </summary>
    /// <param name="jobType">An instance of <see cref="JobType"/> with the desired parameters to update set to not null.</param>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <param name="jobTypeName">Current name of the job type.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit job types in the campaign,
    /// BadRequest if the name given is a built in name, empty, or already exists, Ok otherwise.</returns>
    [HttpPut("update/{campaignGuid:guid}/{jobTypeName}")]
    public async Task<IActionResult> UpdateJobType([FromBody] JobType jobType, Guid campaignGuid, string jobTypeName)
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
                return Unauthorized(FormatErrorMessage(AuthorizationError,
                    CustomStatusCode.AuthorizationError));
            }
            
            if (jobType.JobTypeName != null && BuiltInJobTypes.IsBuiltIn(jobType.JobTypeName))
            {
                return BadRequest(FormatErrorMessage(NameMustNotBeBuiltIn, CustomStatusCode.NameCanNotBeBuiltIn));
            }
            
            var res = await _jobTypesService.UpdateJobType(jobType, campaignGuid, jobTypeName);
            return res switch
            {
                CustomStatusCode.CannotInsertDuplicateUniqueIndex => BadRequest(FormatErrorMessage(ErrorMessages.JobTypeAlreadyExists, res)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating job type");
            return StatusCode(500, "Error while updating job type");
        }
    }
}