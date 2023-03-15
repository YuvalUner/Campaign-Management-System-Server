using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for managing the list of users who can assign others to a specific job.
/// </summary>
public interface IJobAssignmentCapabilityService
{
    /// <summary>
    /// Adds a user to the list of users who can assign others to a specific job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobAssignmentCapabilityParams">An instance of <see cref="JobAssignmentCapabilityParams"/> with the required
    /// properties filled in.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.JobNotFound"/> if the job does not exist,
    /// <see cref="CustomStatusCode.DuplicateKey"/> if the user is already capable of assigning to the job.</returns>
    Task<CustomStatusCode> AddJobAssignmentCapableUser(Guid campaignGuid,
        JobAssignmentCapabilityParams jobAssignmentCapabilityParams);

    /// <summary>
    /// Removes a user from the list of users who can assign others to a specific job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobAssignmentCapabilityParams">An instance of <see cref="JobAssignmentCapabilityParams"/> with the properties
    /// filled in.</param>
    /// <returns></returns>
    Task RemoveJobAssignmentCapableUser(Guid campaignGuid, JobAssignmentCapabilityParams jobAssignmentCapabilityParams);

    /// <summary>
    /// Gets a list of users who can assign others to a specific job.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job belongs to.</param>
    /// <param name="jobGuid">Guid of the job itself.</param>
    /// <param name="viaJobType">True if everyone who can assign to the job (via job type as well) is needed,
    /// False if only those who can assign to that job specifically is needed</param>
    /// <returns>An enumerable of <see cref="User"/> with info about each user.</returns>
    Task<IEnumerable<User>> GetJobAssignmentCapableUsers(Guid campaignGuid, Guid? jobGuid, bool? viaJobType);
}