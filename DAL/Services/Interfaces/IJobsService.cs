using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobsService
{
    /// <summary>
    /// Adds a new job to the campaign.
    /// </summary>
    /// <param name="job"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<Guid> AddJob(Job job, Guid campaignGuid);

    /// <summary>
    /// Deletes a single job from the campaign.
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task DeleteJob(Guid jobGuid, Guid campaignGuid);

    /// <summary>
    /// Updates a single job in the campaign.
    /// Only parameters that are not null will be updated.
    /// Required parameters: JobGuid, CampaignGuid
    /// </summary>
    /// <param name="job"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task UpdateJob(Job job, Guid campaignGuid);

    /// <summary>
    /// Gets a list of all jobs in the campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<Job>> GetJobs(Guid campaignGuid);

    /// <summary>
    /// Gets a single job from the campaign.
    /// </summary>
    /// <param name="jobGuid"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<Job?> GetJob(Guid jobGuid, Guid campaignGuid);
}