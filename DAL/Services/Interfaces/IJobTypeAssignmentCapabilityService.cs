using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobTypeAssignmentCapabilityService
{
    /// <summary>
    /// Adds a new user to the list of users who can assign other users to a job of a specific job type.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobTypeAssignmentCapabilityParams"></param>
    /// <returns>Status code UserNotFound if the user does not exist, JobTypeNotFound if the job type does not exist,
    /// DuplicateKey if the user is already capable of assigning to the job type.</returns>
    Task<CustomStatusCode> AddJobTypeAssignmentCapableUser(Guid campaignGuid, JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams);

    /// <summary>
    /// Removes a user from the list of users who can assign other users to a job of a specific job type.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobTypeAssignmentCapabilityParams"></param>
    /// <returns></returns>
    Task RemoveJobTypeAssignmentCapableUser(Guid campaignGuid, JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams);

    /// <summary>
    /// Gets a list of users who can assign other users to a job of a specific job type.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobTypeName"></param>
    /// <returns></returns>
    Task<IEnumerable<UserPublicInfo>> GetJobTypeAssignmentCapableUsers(Guid campaignGuid, string jobTypeName);
}