using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class JobAssignmentCapabilityService: IJobAssignmentCapabilityService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public JobAssignmentCapabilityService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<CustomStatusCode> AddJobAssignmentCapableUser(Guid campaignGuid, JobAssignmentCapabilityParams jobAssignmentCapabilityParams)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobAssignmentCapabilityParams.JobGuid,
            jobAssignmentCapabilityParams.UserEmail
        });
        param.Add("@returnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddUserWhoCanAssignToJob, param);
        return (CustomStatusCode)param.Get<int>("@returnCode");
    }

    public async Task RemoveJobAssignmentCapableUser(Guid campaignGuid, JobAssignmentCapabilityParams jobAssignmentCapabilityParams)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobAssignmentCapabilityParams.JobGuid,
            jobAssignmentCapabilityParams.UserEmail
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveUserWhoCanAssignToJob, param);
    }

    public async Task<IEnumerable<User>> GetJobAssignmentCapableUsers(Guid campaignGuid, Guid? jobGuid, bool? viaJobType)
    {
        if (viaJobType == null)
            viaJobType = true;
        
        var param = new DynamicParameters(new
        {
            campaignGuid,
            jobGuid,
            viaJobType
        });
        return await _dbAccess.GetData<User, DynamicParameters>(StoredProcedureNames.GetUsersWhoCanAssignToJob, param);
    }
}