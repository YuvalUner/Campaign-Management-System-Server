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
    
    public async Task<IEnumerable<Job>> GetJobsByFilter(Guid campaignGuid, JobsFilterParameters filterParameters)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        if (filterParameters.FullyManned.HasValue)
        {
            param.Add("fullyManned", filterParameters.FullyManned.Value);
        }
        if (filterParameters.JobName != null)
        {
            param.Add("jobName", filterParameters.JobName);
        }
        if (filterParameters.JobTypeName != null)
        {
            param.Add("jobTypeName", filterParameters.JobTypeName);
        }
        if (filterParameters.JobLocation != null)
        {
            param.Add("jobLocation", filterParameters.JobLocation);
        }
        if (filterParameters.JobStartTime.HasValue)
        {
            param.Add("jobStartTime", filterParameters.JobStartTime.Value);
        }
        if (filterParameters.JobEndTime.HasValue)
        {
            param.Add("jobEndTime", filterParameters.JobEndTime.Value);
        }
        if (filterParameters.TimeFromStart.HasValue)
        {
            param.Add("timeFromStart", filterParameters.TimeFromStart.Value);
        }
        if (filterParameters.TimeBeforeStart.HasValue)
        {
            param.Add("timeBeforeStart", filterParameters.TimeBeforeStart.Value);
        }
        if (filterParameters.TimeFromEnd.HasValue)
        {
            param.Add("timeFromEnd", filterParameters.TimeFromEnd.Value);
        }
        if (filterParameters.TimeBeforeEnd.HasValue)
        {
            param.Add("timeBeforeEnd", filterParameters.TimeBeforeEnd.Value);
        }
        if (filterParameters.OnlyCustomJobTypes.HasValue)
        {
            param.Add("onlyCustomJobTypes", filterParameters.OnlyCustomJobTypes.Value);
        }
        return await _dbAccess.GetData<Job, DynamicParameters>(StoredProcedureNames.GetJobsFiltered, param);
    }

    public async Task<CustomStatusCode> AddJobAssignment(Guid campaignGuid, Guid jobGuid, string userEmail, int? salary)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobGuid,
            userEmail,
        });
        if (salary.HasValue)
        {
            param.Add("salary", salary.Value);
        }
        param.Add("statusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await _dbAccess.ModifyData(StoredProcedureNames.AssignToJob, param);
        return (CustomStatusCode) param.Get<int>("statusCode");
    }
    
    public async Task RemoveJobAssignment(Guid campaignGuid, Guid jobGuid, string userEmail)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobGuid,
            userEmail
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveJobAssignment, param);
    }
    
    public async Task<IEnumerable<JobAssignment>> GetJobAssignments(Guid campaignGuid, Guid jobGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobGuid
        });
        return await _dbAccess.GetData<JobAssignment, DynamicParameters>(StoredProcedureNames.GetUsersAssignedToJob, param);
    }
}