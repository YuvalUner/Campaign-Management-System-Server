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

    public async Task<Guid> AddJob(Job job, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            job.JobName,
            job.JobDescription,
            job.JobLocation,
            job.JobStartTime,
            job.JobEndTime,
            job.JobDefaultSalary
        });
        param.Add("jobGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        await _dbAccess.ModifyData(StoredProcedureNames.AddJob, param);
        return param.Get<Guid>("jobGuid");
    }
}