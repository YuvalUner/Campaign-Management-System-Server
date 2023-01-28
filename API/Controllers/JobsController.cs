using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class JobsController : Controller
{
    private readonly IJobsService _jobsService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobsService jobsService, ILogger<JobsController> logger)
    {
        _jobsService = jobsService;
        _logger = logger;
    }

    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddJob([FromBody] Job job, Guid campaignGuid)
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
            
            if (string.IsNullOrWhiteSpace(job.JobName))
            {
                return BadRequest(FormatErrorMessage(JobNameRequired, CustomStatusCode.ValueCanNotBeNull));
            }
            
            if (job.PeopleNeeded <= 0)
            {
                return BadRequest(FormatErrorMessage(JobRequiresPeople, CustomStatusCode.IllegalValue));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var jobGuid = await _jobsService.AddJob(job, campaignGuid, userId);
            return Ok(jobGuid);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding job");
            return StatusCode(500, "Error while adding job");
        }
    }
    
    [HttpDelete("delete/{campaignGuid:guid}/{jobGuid}")]
    public async Task<IActionResult> DeleteJob(Guid jobGuid, Guid campaignGuid)
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
            
            await _jobsService.DeleteJob(jobGuid, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting job");
            return StatusCode(500, "Error while deleting job");
        }
    }
    
    [HttpPut("update/{campaignGuid:guid}/{jobGuid:guid}")]
    public async Task<IActionResult> UpdateJob([FromBody] Job job, Guid campaignGuid, Guid jobGuid)
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

            job.JobGuid = jobGuid;
            
                await _jobsService.UpdateJob(job, campaignGuid);
                return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating job");
            return StatusCode(500, "Error while updating job");
        }
    }
    
    [HttpGet("get/{campaignGuid:guid}")]
    public async Task<IActionResult> GetJobs(Guid campaignGuid)
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
            
            var jobs = await _jobsService.GetJobs(campaignGuid);

            return Ok(jobs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting jobs");
            return StatusCode(500, "Error while getting jobs");
        }
    }
    
    [HttpGet("get/{campaignGuid:guid}/{jobGuid:guid}")]
    public async Task<IActionResult> GetJob(Guid campaignGuid, Guid jobGuid)
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
            
            var job = await _jobsService.GetJob(jobGuid, campaignGuid);
            if (job == null)
            {
                return NotFound(FormatErrorMessage(JobNotFound, CustomStatusCode.ValueNotFound));
            }
            return Ok(job);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting job");
            return StatusCode(500, "Error while getting job");
        }
    }
    
    [HttpGet("get-by-manned-status/{campaignGuid:guid}/{fullyManned:bool}")]
    public async Task<IActionResult> GetJobsByMannedStatus(Guid campaignGuid, bool fullyManned)
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
            
            var jobs = await _jobsService.GetsJobsByMannedStatus(campaignGuid, fullyManned);

            return Ok(jobs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting jobs by manned status");
            return StatusCode(500, "Error while getting jobs by manned status");
        }
    }
    
}