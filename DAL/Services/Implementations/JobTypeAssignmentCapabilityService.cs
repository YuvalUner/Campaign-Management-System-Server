using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;



public class JobTypeAssignmentCapabilityService: IJobTypeAssignmentCapabilityService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public JobTypeAssignmentCapabilityService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    public async Task<CustomStatusCode> AddJobTypeAssignmentCapableUser(Guid campaignGuid,
        JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobTypeAssignmentCapabilityParams.JobTypeName,
            jobTypeAssignmentCapabilityParams.UserEmail
        });
        param.Add("@returnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddUserWhoCanAssignToJobType, param);
        return (CustomStatusCode)param.Get<int>("@returnCode");
    }
    
    public async Task RemoveJobTypeAssignmentCapableUser(Guid campaignGuid,
        JobTypeAssignmentCapabilityParams jobTypeAssignmentCapabilityParams)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobTypeAssignmentCapabilityParams.JobTypeName,
            jobTypeAssignmentCapabilityParams.UserEmail
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveUserWhoCanAssignToJobType, param);
    }
    
    public async Task<IEnumerable<User>> GetJobTypeAssignmentCapableUsers(Guid campaignGuid, string jobTypeName)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobTypeName
        });
        return await _dbAccess.GetData<User, DynamicParameters>(StoredProcedureNames.GetUsersWhoCanAssignToJobType, param);
    }
}