using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for managing which users can assign other users to jobs of a specific job type.
/// </summary>
public interface IJobTypeAssignmentCapabilityService
{
    /// <summary>
    /// Adds a new user to the list of users who can assign other users to a job of a specific job type.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <param name="jobTypeAssignmentCapabilityParams">An instance of <see cref="JobTypeAssignmentCapabilityParams"/>
    /// with all the properties not null.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.JobTypeNotFound"/> if the job type does not exist,
    /// <see cref="CustomStatusCode.DuplicateKey"/> if the user is already capable of assigning to the job type.</returns>
    Task<CustomStatusCode> AddJobTypeAssignmentCapableUser(Guid campaignGuid, JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams);

    /// <summary>
    /// Removes a user from the list of users who can assign other users to a job of a specific job type.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <param name="jobTypeAssignmentCapabilityParams">An instance of <see cref="JobTypeAssignmentCapabilityParams"/>
    /// with all properties set to not null.</param>
    /// <returns></returns>
    Task RemoveJobTypeAssignmentCapableUser(Guid campaignGuid, JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams);

    /// <summary>
    /// Gets a list of users who can assign other users to a job of a specific job type.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="jobTypeName">Name of the job type.</param>
    /// <returns>A list of <see cref="User"/>s who can assign to that job type.</returns>
    Task<IEnumerable<User>> GetJobTypeAssignmentCapableUsers(Guid campaignGuid, string jobTypeName);
}