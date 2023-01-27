using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                return Unauthorized();
            }
            
            if (string.IsNullOrWhiteSpace(job.JobName))
            {
                return BadRequest("Job name is required");
            }

            var jobGuid = await _jobsService.AddJob(job, campaignGuid);
            return Ok(jobGuid);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding job");
            return StatusCode(500, "Error while adding job");
        }
    }
}