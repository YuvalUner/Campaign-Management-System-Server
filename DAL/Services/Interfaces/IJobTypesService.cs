using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for CRUD operations on job types.
/// </summary>
public interface IJobTypesService
{
    /// <summary>
    /// Adds a new job type to the campaign.
    /// </summary>
    /// <param name="jobType">An instance of <see cref="JobType"/> with all the required properties filled in.</param>
    /// <param name="campaignGuid">Guid of the campaign to add the job type for.</param>
    /// <param name="userId">User id of the user creating the job type.</param>
    /// <returns><see cref="CustomStatusCode.TooManyEntries"/> if there are already 40 job types for the campaign,
    /// <see cref="CustomStatusCode.CannotInsertDuplicateUniqueIndex"/> if job type name already exists for that campaign</returns>
    Task<CustomStatusCode> AddJobType(JobType jobType, Guid campaignGuid, int? userId);
    
    /// <summary>
    /// Deletes a single job type from the campaign.
    /// </summary>
    /// <param name="jobTypeName">Name of the job type.</param>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <returns></returns>
    Task DeleteJobType(string jobTypeName, Guid campaignGuid);
    
    /// <summary>
    /// Updates a single job type in the campaign.
    /// Only parameters that are not null will be updated.
    /// Required parameters: JobTypeName, CampaignGuid
    /// </summary>
    /// <param name="jobType">An instance of <see cref="JobType"/> with the parameters that should be updated filled in.</param>
    /// <param name="campaignGuid">Guid of the campaign the job type belongs to.</param>
    /// <param name="jobTypeName">The current name of the job type.</param>
    /// <returns><see cref="CustomStatusCode.CannotInsertDuplicateUniqueIndex"/> if a job type with that name already exists
    /// in the campaign.</returns>
    Task<CustomStatusCode> UpdateJobType(JobType jobType, Guid campaignGuid, string jobTypeName);
    
    /// <summary>
    /// Gets a list of all job types in the campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>A list of <see cref="JobType"/>.</returns>
    Task<IEnumerable<JobType>> GetJobTypes(Guid campaignGuid);
}