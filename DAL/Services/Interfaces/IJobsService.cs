using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for managing and retrieving information related to jobs and job assignments.
/// </summary>
public interface IJobsService
{
    /// <summary>
    /// Adds a new job to the campaign.
    /// </summary>
    /// <param name="job">An instance of <see cref="Job"/> with the required properties filled in.</param>
    /// <param name="campaignGuid">Guid of the campaign to add the job for.</param>
    /// <param name="userId">Id of the user who is creating the job.</param>
    /// <returns>The Guid of the newly created job</returns>
    Task<Guid> AddJob(Job job, Guid campaignGuid, int? userId);

    /// <summary>
    /// Deletes a single job from the campaign.
    /// </summary>
    /// <param name="jobGuid">Guid of the job to delete.</param>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <returns></returns>
    Task DeleteJob(Guid jobGuid, Guid campaignGuid);

    /// <summary>
    /// Updates a single job in the campaign.
    /// Only parameters that are not null will be updated.
    /// Required parameters: JobGuid, CampaignGuid
    /// </summary>
    /// <param name="job">An instance of <see cref="Job"/> with any field that should be updated filled in.</param>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <returns></returns>
    Task UpdateJob(Job job, Guid campaignGuid);

    /// <summary>
    /// Gets a list of all jobs in the campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get jobs for.</param>
    /// <returns>An enumerable of <see cref="Job"/> with information about every job.</returns>
    Task<IEnumerable<Job>> GetJobs(Guid campaignGuid);

    /// <summary>
    /// Gets a single job from the campaign.
    /// </summary>
    /// <param name="jobGuid">Guid of the job.</param>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <returns>A single instance of <see cref="Job"/> with the properties filled in, or null if it does not exist.</returns>
    Task<Job?> GetJob(Guid jobGuid, Guid campaignGuid);

    /// <summary>
    /// Gets a list of jobs in the campaign that match the filter parameters.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get jobs for.</param>
    /// <param name="filterParameters">An instance of <see cref="JobsFilterParameters"/> where every non null property
    /// is one that should be filtered by.</param>
    /// <returns>A filtered list of <see cref="Job"/>s.</returns>
    Task<IEnumerable<Job>> GetJobsByFilter(Guid campaignGuid, JobsFilterParameters filterParameters);

    /// <summary>
    /// Gets the list of users assigned to a job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <returns>A list of <see cref="JobAssignment"/>, each entry containing information about each user that is assigned
    /// to the job.</returns>
    Task<IEnumerable<JobAssignment>> GetJobAssignments(Guid campaignGuid, Guid jobGuid);

    /// <summary>
    /// Removes a user from a job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <param name="userEmail">Email of the user to remove from the job.</param>
    /// <returns>Status code UserNotFound if the user does not exist, JobNotFound if the job does not exist.</returns>
    Task<CustomStatusCode> RemoveJobAssignment(Guid campaignGuid, Guid jobGuid, string userEmail);

    /// <summary>
    /// Adds a user to a job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <param name="jobAssignmentParams"></param>
    /// <param name="assigningUserId"></param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.JobNotFound"/> if the job does not exist,
    /// <see cref="CustomStatusCode.JobFullyManned"/> if the job is already fully manned,
    /// <see cref="CustomStatusCode.DuplicateKey"/> if user is already assigned to job.</returns>
    Task<CustomStatusCode> AddJobAssignment(Guid campaignGuid, Guid jobGuid,
        JobAssignmentParams jobAssignmentParams, int? assigningUserId);

    /// <summary>
    /// Updates the salary of a user assigned to a job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <param name="jobAssignmentParams">An instance of <see cref="JobAssignmentParams"/> with the requires properties
    /// not null</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.JobNotFound"/> if the job does not exist</returns>
    Task<CustomStatusCode> UpdateJobAssignment(Guid campaignGuid, Guid jobGuid,
        JobAssignmentParams jobAssignmentParams);

    /// <summary>
    /// Gets a list of jobs that a user is assigned to.
    /// </summary>
    /// <param name="userId">User id of the user to get jobs for.</param>
    /// <param name="campaignGuid">The campaign's guid. Keep as null to get all jobs user is assigned to from all campaigns,
    /// along with their campaign name and Guid.</param>
    /// <returns>A list of <see cref="UserJob"/> that is relevant to the user.</returns>
    Task<IEnumerable<UserJob>> GetUserJobs(int? userId, Guid? campaignGuid = null);

    /// <summary>
    /// Gets a list of users that are available for a job.
    /// Filtering can also be done, according to the parameters in the filterParams object.
    /// </summary>
    /// <param name="filterParams">An instance of <see cref="UsersFilterForJobsParams"/> with all properties that should
    /// be filtered by not null.</param>
    /// <returns>A list of <see cref="UsersFilterResults"/> that matches the filters.</returns>
    Task<IEnumerable<UsersFilterResults>> GetUsersAvailableForJob(UsersFilterForJobsParams filterParams);
}