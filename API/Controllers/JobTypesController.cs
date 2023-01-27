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

            await _jobTypesService.AddJobType(jobType, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding job type");
            return StatusCode(500, "Error while adding job type");
        }
    }
}