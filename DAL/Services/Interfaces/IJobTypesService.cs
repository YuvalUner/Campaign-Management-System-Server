using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobTypesService
{
    /// <summary>
    /// Adds a new job type to the campaign.
    /// </summary>
    /// <param name="jobType"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="userId"></param>
    /// <returns>TooManyEntries if there are already 40 job types for the campaign,
    /// CannotInsertDuplicateUniqueIndex if job type name already exists for that campaign</returns>
    Task<CustomStatusCode> AddJobType(JobType jobType, Guid campaignGuid, int? userId);
    
    /// <summary>
    /// Deletes a single job type from the campaign.
    /// </summary>
    /// <param name="jobTypeName"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task DeleteJobType(string jobTypeName, Guid campaignGuid);
    
    /// <summary>
    /// Updates a single job type in the campaign.
    /// Only parameters that are not null will be updated.
    /// Required parameters: JobTypeName, CampaignGuid
    /// </summary>
    /// <param name="jobType"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="jobTypeName"></param>
    /// <returns></returns>
    Task<CustomStatusCode> UpdateJobType(JobType jobType, Guid campaignGuid, string jobTypeName);
    
    /// <summary>
    /// Gets a list of all job types in the campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<JobType>> GetJobTypes(Guid campaignGuid);
}