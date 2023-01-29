﻿using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobsService
{
    /// <summary>
    /// Adds a new job to the campaign.
    /// </summary>
    /// <param name="job"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="userId"></param>
    /// <returns>The Guid of the newly created job</returns>
    Task<Guid> AddJob(Job job, Guid campaignGuid, int? userId);

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

    /// <summary>
    /// Gets a list of all jobs in the campaign that are fully manned or not fully manned, depending on the value of fullyManned.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="fullyManned"></param>
    /// <param name="filterParameters"></param>
    /// <returns></returns>
    Task<IEnumerable<Job>> GetJobsByFilter(Guid campaignGuid, JobsFilterParameters filterParameters);

    /// <summary>
    /// Gets the list of users assigned to a job.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<JobAssignment>> GetJobAssignments(Guid campaignGuid, Guid jobGuid);

    /// <summary>
    /// Removes a user from a job.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobGuid"></param>
    /// <param name="userEmail"></param>
    /// <returns></returns>
    Task<CustomStatusCode> RemoveJobAssignment(Guid campaignGuid, Guid jobGuid, string userEmail);

    /// <summary>
    /// Adds a user to a job.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobGuid"></param>
    /// <param name="jobAssignmentParams"></param>
    /// <returns></returns>
    Task<CustomStatusCode> AddJobAssignment(Guid campaignGuid, Guid jobGuid, JobAssignmentParams jobAssignmentParams);

    /// <summary>
    /// Updates the salary of a user assigned to a job.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="jobGuid"></param>
    /// <param name="jobAssignmentParams"></param>
    /// <returns></returns>
    Task<CustomStatusCode> UpdateJobAssignment(Guid campaignGuid, Guid jobGuid,
        JobAssignmentParams jobAssignmentParams);
}