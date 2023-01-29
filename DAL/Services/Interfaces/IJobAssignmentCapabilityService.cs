using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobAssignmentCapabilityService
{
    /// <summary>
    /// Adds a user to the list of users who can assign others to a specific job.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobAssignmentCapabilityParams"></param>
    /// <returns>Status code UserNotFound if the user does not exist, JobNotFound if the job does not exist,
    /// DuplicateKey if the user is already capable of assigning to the job.</returns>
    Task<CustomStatusCode> AddJobAssignmentCapableUser(Guid campaignGuid, JobAssignmentCapabilityParams jobAssignmentCapabilityParams);

    /// <summary>
    /// Removes a user from the list of users who can assign others to a specific job.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobAssignmentCapabilityParams"></param>
    /// <returns></returns>
    Task RemoveJobAssignmentCapableUser(Guid campaignGuid, JobAssignmentCapabilityParams jobAssignmentCapabilityParams);
    
    /// <summary>
    /// Gets a list of users who can assign others to a specific job.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobGuid"></param>
    /// <param name="viaJobType">True if everyone who can assign to the job (via job type as well) is needed,
    /// False if only those who can assign to that job specifically is needed</param>
    /// <returns></returns>
    Task<IEnumerable<User>> GetJobAssignmentCapableUsers(Guid campaignGuid, Guid? jobGuid, bool? viaJobType);
}