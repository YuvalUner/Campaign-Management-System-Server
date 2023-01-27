using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobsService
{
    Task<Guid> AddJob(Job job, Guid campaignGuid);
}