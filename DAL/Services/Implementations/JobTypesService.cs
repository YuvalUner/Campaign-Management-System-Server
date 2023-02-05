using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class JobTypesService: IJobTypesService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public JobTypesService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    public async Task<CustomStatusCode> AddJobType(JobType jobType, Guid campaignGuid, int? userId)
    {
        var param = new DynamicParameters(new
        {
            jobType.JobTypeName,
            jobType.JobTypeDescription,
            campaignGuid,
            userId
        });
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddJobType, param);
        return (CustomStatusCode) param.Get<int>("returnVal");
    }
    
    public async Task DeleteJobType(string jobTypeName, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        param.Add("jobTypeName", jobTypeName, DbType.String, ParameterDirection.Input);
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteJobType, param);
    }
    
    public async Task<CustomStatusCode> UpdateJobType(JobType jobType, Guid campaignGuid, string jobTypeName)
    {
        var param = new DynamicParameters(new
        {
            newJobTypeName = jobType.JobTypeName,
            newJobTypeDescription = jobType.JobTypeDescription,
            campaignGuid,
        });
        // Adding this later, as the name may be in hebrew and specifying it this way guarantees handling as nvarchar
        param.Add("jobTypeName", jobTypeName, DbType.String, ParameterDirection.Input);
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateJobType, param);
        return (CustomStatusCode) param.Get<int>("returnVal");
    }
    
    public async Task<IEnumerable<JobType>> GetJobTypes(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        return await _dbAccess.GetData<JobType, DynamicParameters>(StoredProcedureNames.GetJobTypes, param);
    }
}