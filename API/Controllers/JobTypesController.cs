using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static API.Utils.ErrorMessages;
using StatusCodes = DAL.DbAccess.StatusCodes;

namespace API.Controllers;

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
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(jobType.JobTypeName))
            {
                return BadRequest("Job type name cannot be empty");
            }

            if (BuiltInJobTypes.IsBuiltIn(jobType.JobTypeName))
            {
                return BadRequest("Job type name cannot be a built-in job type name");
            }
            
            var res = await _jobTypesService.AddJobType(jobType, campaignGuid);
            return res switch
            {
                StatusCodes.CannotInsertDuplicateUniqueIndex => BadRequest(FormatErrorMessage(ErrorMessages.JobTypeAlreadyExists, res)),
                StatusCodes.TooManyEntries => BadRequest($"Error Num {res} - Too many job types"),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding job type");
            return StatusCode(500, "Error while adding job type");
        }
    }

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
                return Unauthorized();
            }
            
            if (BuiltInJobTypes.IsBuiltIn(jobTypeName))
            {
                return BadRequest("Cannot delete built-in job type");
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
                return Unauthorized();
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
                return Unauthorized();
            }
            
            if (jobType.JobTypeName != null && BuiltInJobTypes.IsBuiltIn(jobType.JobTypeName))
            {
                return BadRequest("Job type name cannot be a built-in job type name");
            }
            
            var res = await _jobTypesService.UpdateJobType(jobType, campaignGuid, jobTypeName);
            return res switch
            {
                StatusCodes.CannotInsertDuplicateUniqueIndex => BadRequest(FormatErrorMessage(ErrorMessages.JobTypeAlreadyExists, res)),
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