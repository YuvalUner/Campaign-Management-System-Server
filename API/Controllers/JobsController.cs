using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RestAPIServices;
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
    private readonly IJobAssignmentCapabilityService _jobAssignmentCapabilityService;
    private readonly ISmsMessageSendingService _smsMessageSendingService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly IUsersService _usersService;

    public JobsController(IJobsService jobsService, ILogger<JobsController> logger,
        IJobAssignmentCapabilityService jobAssignmentCapabilityService, 
        ISmsMessageSendingService smsMessageSendingService, IEmailSendingService emailSendingService,
        IUsersService usersService)
    {
        _jobsService = jobsService;
        _logger = logger;
        _jobAssignmentCapabilityService = jobAssignmentCapabilityService;
        _smsMessageSendingService = smsMessageSendingService;
        _emailSendingService = emailSendingService;
        _usersService = usersService;
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
    
    [HttpGet("get-filtered/{campaignGuid:guid}")]
    public async Task<IActionResult> GetJobsByMannedStatus(Guid campaignGuid, [FromQuery] JobsFilterParameters filterParameters)
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
            
            var jobs = await _jobsService.GetJobsByFilter(campaignGuid, filterParameters);

            return Ok(jobs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting jobs by manned status");
            return StatusCode(500, "Error while getting jobs by manned status");
        }
    }
    
    [HttpPost("assign/{campaignGuid:guid}/{jobGuid:guid}")]
    public async Task<IActionResult> AssignJob(Guid campaignGuid, Guid jobGuid,
        [FromBody] JobAssignmentParams jobAssignmentParams, [FromQuery] bool sendNotification = true)
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


            if (!await JobAssignmentUtils.CanModifyJobAssignment(_jobAssignmentCapabilityService, HttpContext, jobGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(NoPermissionToAssignToJob, CustomStatusCode.PermissionError));
            }

            if (string.IsNullOrWhiteSpace(jobAssignmentParams.UserEmail))
            {
                return BadRequest(FormatErrorMessage(EmailNullOrEmpty, CustomStatusCode.ValueCanNotBeNull));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var res = await _jobsService.AddJobAssignment(campaignGuid, jobGuid, jobAssignmentParams, userId);
            switch (res)
            {
                case CustomStatusCode.JobNotFound:
                    return NotFound(FormatErrorMessage(JobNotFound, res));
                case CustomStatusCode.UserNotFound:
                    return NotFound(FormatErrorMessage(UserNotFound, res));
                case CustomStatusCode.JobFullyManned:
                    return BadRequest(FormatErrorMessage(JobFullyManned, res));
                case CustomStatusCode.DuplicateKey:
                    return BadRequest(FormatErrorMessage(AlreadyAssignedToJob, res));
            }

            if (sendNotification)
            {
                var contactInfo = await _usersService.GetUserContactInfoByEmail(jobAssignmentParams.UserEmail);
                var job = await _jobsService.GetJob(jobGuid, campaignGuid);
                if (contactInfo != null && job != null)
                {
                    if (contactInfo.PhoneNumber != null)
                    {
                        _smsMessageSendingService.SendJobAssignedSmsAsync(job.JobName, job.JobStartTime, job.JobEndTime,
                            job.JobLocation, contactInfo.PhoneNumber, CountryCodes.Israel);
                    }
                    if (contactInfo.Email != null)
                    {
                        _emailSendingService.SendJobAssignedEmailAsync(job.JobName, job.JobStartTime, job.JobEndTime,
                            job.JobLocation, contactInfo.Email);
                    }
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while assigning job");
            return StatusCode(500, "Error while assigning job");
        }
    }
    
    [HttpDelete("unassign/{campaignGuid:guid}/{jobGuid:guid}")]
    public async Task<IActionResult> UnassignJob(Guid campaignGuid, Guid jobGuid,
        [FromBody] JobAssignmentParams jobAssignmentParams, [FromQuery] bool sendNotification = true)
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
            
            // While the method is called CanModifyJobAssignment, the logic here is that if the user can't assign to the job,
            // they can't unassign from it either.
            if (!await JobAssignmentUtils.CanModifyJobAssignment(_jobAssignmentCapabilityService, HttpContext, jobGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(NoPermissionToAssignToJob, CustomStatusCode.PermissionError));
            }

            if (string.IsNullOrWhiteSpace(jobAssignmentParams.UserEmail))
            {
                return BadRequest(FormatErrorMessage(EmailNullOrEmpty, CustomStatusCode.ValueCanNotBeNull));
            }
            
            var res = await _jobsService.RemoveJobAssignment(campaignGuid, jobGuid, jobAssignmentParams.UserEmail);
            switch (res)
            {
                case CustomStatusCode.UserNotFound:
                    return NotFound(FormatErrorMessage(UserNotFound, res));
                case CustomStatusCode.JobNotFound:
                    return NotFound(FormatErrorMessage(JobNotFound, res));
            }
            if (sendNotification)
            {
                var contactInfo = await _usersService.GetUserContactInfoByEmail(jobAssignmentParams.UserEmail);
                var job = await _jobsService.GetJob(jobGuid, campaignGuid);
                if (contactInfo != null && job != null)
                {
                    if (contactInfo.PhoneNumber != null)
                    {
                        _smsMessageSendingService.SendJobUnAssignedSmsAsync(job.JobName,
                            job.JobLocation, contactInfo.PhoneNumber, CountryCodes.Israel);
                    }
                    if (contactInfo.Email != null)
                    {
                        _emailSendingService.SendJobUnAssignedEmailAsync(job.JobName,
                            job.JobLocation, contactInfo.Email);
                    }
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while unassigning job");
            return StatusCode(500, "Error while unassigning job");
        }
    }
    
    [HttpGet("get-assignments/{campaignGuid:guid}/{jobGuid:guid}")]
    public async Task<IActionResult> GetJobAssignments(Guid campaignGuid, Guid jobGuid)
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
            
            var jobAssignments = await _jobsService.GetJobAssignments(campaignGuid, jobGuid);
            return Ok(jobAssignments);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting job assignments");
            return StatusCode(500, "Error while getting job assignments");
        }
    }
    
    [HttpPut("update-assignment/{campaignGuid:guid}/{jobGuid:guid}")]
    public async Task<IActionResult> UpdateJobAssignment(Guid campaignGuid, Guid jobGuid, [FromBody] JobAssignmentParams jobAssignmentParams)
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
            
            if (!await JobAssignmentUtils.CanModifyJobAssignment(_jobAssignmentCapabilityService, HttpContext,
                    jobGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(NoPermissionToAssignToJob, CustomStatusCode.PermissionError));
            }

            if (string.IsNullOrWhiteSpace(jobAssignmentParams.UserEmail))
            {
                return BadRequest(FormatErrorMessage(EmailNullOrEmpty, CustomStatusCode.ValueNullOrEmpty));
            }
            
            if (jobAssignmentParams.Salary == null || jobAssignmentParams.Salary < 0)
            {
                return BadRequest(FormatErrorMessage(SalaryNullOrEmpty, CustomStatusCode.ValueNullOrEmpty));
            }
            
            var res = await _jobsService.UpdateJobAssignment(campaignGuid, jobGuid, jobAssignmentParams);
            return res switch
            {
                CustomStatusCode.JobNotFound => NotFound(FormatErrorMessage(JobNotFound, res)),
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound, res)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating job assignment");
            return StatusCode(500, "Error while updating job assignment");
        }
    }

    [HttpGet("get-self-jobs")]
    public async Task<IActionResult> GetSelfJobs([FromQuery] Guid? campaignGuid)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            var jobs = await _jobsService.GetUserJobs(userId, campaignGuid);
            return Ok(jobs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting self jobs");
            return StatusCode(500, "Error while getting self jobs");
        }
    }
    
    [HttpGet("get-available-users/{campaignGuid:guid}")]
    public async Task<IActionResult> GetAvailableUsers(Guid campaignGuid, [FromQuery] UsersFilterForJobsParams filterParams)
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
            if (filterParams.JobStartTime == null && filterParams.JobEndTime == null)
            {
                return BadRequest(FormatErrorMessage(TimeframeMustBeProvided, CustomStatusCode.ValueNullOrEmpty));
            }
            
            filterParams.CampaignGuid = campaignGuid;
            
            var users = await _jobsService.GetUsersAvaialbleForJob(filterParams);
            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting available users");
            return StatusCode(500, "Error while getting available users");
        }
    }
    
}