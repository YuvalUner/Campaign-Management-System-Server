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
    
    public async Task AddJobType(JobType jobType, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            jobType.JobTypeName,
            jobType.JobTypeDescription,
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.AddJobType, param);
    }
    
    public async Task DeleteJobType(string jobTypeName, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            jobTypeName,
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteJobType, param);
    }
    
    public async Task UpdateJobType(JobType jobType, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            jobType.JobTypeName,
            jobType.JobTypeDescription,
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateJobType, param);
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