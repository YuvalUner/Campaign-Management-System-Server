using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;
using static API.Utils.ErrorMessages;
#pragma warning disable CS4014

namespace API.Controllers;

/// <summary>
/// A controller for jobs and job assignment related actions.<br/>
/// Provides a web API and service policy for <see cref="IJobsService"/> methods.
/// </summary>
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

    /// <summary>
    /// Adds a new job to a campaign.
    /// </summary>
    /// <param name="job">An instance of <see cref="Job"/> with its required fields filled in.</param>
    /// <param name="campaignGuid">Guid of the campaign to add the job to.</param>
    /// <returns>Unauthorized if the user does not have permission to edit jobs in the campaign,
    /// BadRequest if the name is null or empty or the peopleNeeded property is less than 0,
    /// Ok with the Guid of the new job otherwise.</returns>
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
            return Ok(new { jobGuid });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding job");
            return StatusCode(500, "Error while adding job");
        }
    }

    /// <summary>
    /// Deletes an existing job from a campaign.
    /// </summary>
    /// <param name="jobGuid">Guid of the job to delete.</param>
    /// <param name="campaignGuid">Guid of the campaign the job is a part of.</param>
    /// <returns>Unauthorized if the user does not have permission to edit jobs in the campaign,
    /// Ok otherwise.</returns>
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

    /// <summary>
    /// Updates an existing job in a campaign.
    /// </summary>
    /// <param name="job">An instance of <see cref="Job"/> where the fields that should be updated are not null.</param>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <returns>Unauthorized if the user does not have permission to edit jobs in the campaign,
    /// BadRequest if the name is null or empty or the peopleNeeded property is less than 0,
    /// Ok otherwise.</returns>
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

            if (string.IsNullOrWhiteSpace(job.JobName))
            {
                return BadRequest(FormatErrorMessage(JobNameRequired, CustomStatusCode.ValueCanNotBeNull));
            }

            if (job.PeopleNeeded <= 0)
            {
                return BadRequest(FormatErrorMessage(JobRequiresPeople, CustomStatusCode.IllegalValue));
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

    /// <summary>
    /// Gets all jobs for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>Unauthorized if the user does not have permission to view jobs in the campaign,
    /// Ok with a list of <see cref="Job"/> otherwise.</returns>
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

    /// <summary>
    /// Gets a specific job from a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <returns>Unauthorized if the user does not have permission to view jobs in the campaign,
    /// NotFound if the job does not exist,Ok with the job otherwise.</returns>
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

    /// <summary>
    /// Gets all jobs within a campaign that match the filter parameters.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="filterParameters">An instance of <see cref="JobsFilterParameters"/> where all the parameters to filter
    /// by are not null, and the rest are null.</param>
    /// <returns>Unauthorized if the user does not have permission to view jobs in the campaign,
    /// A list of <see cref="Job"/> that match the filter otherwise.</returns>
    [HttpGet("get-filtered/{campaignGuid:guid}")]
    public async Task<IActionResult> GetJobsByMannedStatus(Guid campaignGuid,
        [FromQuery] JobsFilterParameters filterParameters)
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

    /// <summary>
    /// Assigns a person to a job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <param name="jobAssignmentParams">An instance of <see cref="JobAssignmentParams"/> with the email of the
    /// person to assign, and optionally the salary to give them.</param>
    /// <param name="sendNotification">Whether to send the user a notification via sms and email about their assignment
    /// or not.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit jobs in the campaign or if they are
    /// not authorized to assign to the job, BadRequest if the provided email is null or empty or the job is already
    /// fully manned or the user is already assigned to the job, NotFound if the job or user do not exist,
    /// Ok otherwise.
    /// </returns>
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


            if (!await JobAssignmentUtils.CanModifyJobAssignment(_jobAssignmentCapabilityService, HttpContext, jobGuid,
                    campaignGuid))
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <param name="jobAssignmentParams">An instance of <see cref="JobAssignmentParams"/> with the email of the
    /// person to assign.</param>
    /// <param name="sendNotification">Whether to send the user a notification via sms and email about their un-assignment
    /// or not.</param>
    /// <returns>Unauthorized if the requesting user does not have permission to edit jobs in the campaign or if they are
    /// not authorized to assign to the job, BadRequest if the provided email is null or empty,
    /// NotFound if the job or user do not exist, Ok otherwise.
    /// </returns>
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
            if (!await JobAssignmentUtils.CanModifyJobAssignment(_jobAssignmentCapabilityService, HttpContext, jobGuid,
                    campaignGuid))
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

    /// <summary>
    /// Gets all the people assigned to a job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <returns>Unauthorized if the user does not have permission to view jobs in the campaign,
    /// Ok with a list of <see cref="JobAssignment"/> otherwise.</returns>
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

    /// <summary>
    /// Updates the assignment of a job, allowing the update of the salary paid to the assigned person.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <param name="jobAssignmentParams">An instance of <see cref="JobAssignmentParams"/> with all properties filled in.</param>
    /// <returns>Unauthorized if the user does not have permission to edit jobs in the campaign or if they are not
    /// authorized to assign to the job, BadRequest if the provided email is null or empty or the salary is less than 0,
    /// NotFound if the job or user do not exist, Ok otherwise.
    /// </returns>
    [HttpPut("update-assignment/{campaignGuid:guid}/{jobGuid:guid}")]
    public async Task<IActionResult> UpdateJobAssignment(Guid campaignGuid, Guid jobGuid,
        [FromBody] JobAssignmentParams jobAssignmentParams)
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

            if (jobAssignmentParams.Salary is null or < 0)
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

    /// <summary>
    /// Gets the list of jobs for the current user within the given campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get the jobs for.</param>
    /// <returns>Ok with a list of <see cref="UserJob"/>.</returns>
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

    /// <summary>
    /// Allows for filtering of users before assigning them to a job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the users are in.</param>
    /// <param name="filterParams">An instance of <see cref="UsersFilterForJobsParams"/> with the required filter values
    /// set to not null.</param>
    /// <returns>Unauthorized if the user does not have permission to view jobs in the campaign,
    /// BadRequest if no timeframe (either start time or end time) are provided,
    /// Ok with a list of <see cref="UsersFilterResults"/> otherwise.</returns>
    [HttpPost("get-available-users/{campaignGuid:guid}")]
    public async Task<IActionResult> GetAvailableUsers(Guid campaignGuid,
        [FromBody] UsersFilterForJobsParams filterParams)
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

            var users = await _jobsService.GetUsersAvailableForJob(filterParams);
            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting available users");
            return StatusCode(500, "Error while getting available users");
        }
    }
}