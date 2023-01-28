using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class JobsService: IJobsService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public JobsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Guid> AddJob(Job job, Guid campaignGuid, int? userId)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            job.JobName,
            job.JobDescription,
            job.JobLocation,
            job.JobStartTime,
            job.JobEndTime,
            job.JobDefaultSalary,
            job.PeopleNeeded,
            job.JobTypeName,
            userId
        });
        param.Add("newJobGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        await _dbAccess.ModifyData(StoredProcedureNames.AddJob, param);
        return param.Get<Guid>("newJobGuid");
    }
    
    public async Task DeleteJob(Guid jobGuid, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            jobGuid,
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteJob, param);
    }
    
    public async Task UpdateJob(Job job, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            job.JobGuid,
            campaignGuid,
            job.JobName,
            job.JobDescription,
            job.JobLocation,
            job.JobStartTime,
            job.JobEndTime,
            job.JobDefaultSalary,
            job.PeopleNeeded,
            job.JobTypeName
        });
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateJob, param);
    }
    
    public async Task<IEnumerable<Job>> GetJobs(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        return await _dbAccess.GetData<Job, DynamicParameters>(StoredProcedureNames.GetJobs, param);
    }
    
    public async Task<Job?> GetJob(Guid jobGuid, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            jobGuid,
            campaignGuid
        });
        var res =  await _dbAccess.GetData<Job, DynamicParameters>(StoredProcedureNames.GetJob, param);
        return res.FirstOrDefault();
    }
    
    public async Task<IEnumerable<Job>> GetsJobsByMannedStatus(Guid campaignGuid, bool fullyManned)
    {
        var param = new DynamicParameters(new
        {
            fullyManned,
            campaignGuid
        });
        return await _dbAccess.GetData<Job, DynamicParameters>(StoredProcedureNames.GetJobsByMannedStatus, param);
    }
}