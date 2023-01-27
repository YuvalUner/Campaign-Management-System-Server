using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobTypesService
{
    Task AddJobType(JobType jobType, Guid campaignGuid);
    Task DeleteJobType(string jobTypeName, Guid campaignGuid);
    Task UpdateJobType(JobType jobType, Guid campaignGuid);
    Task<IEnumerable<JobType>> GetJobTypes(Guid campaignGuid);
}