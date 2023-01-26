using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class RolesService : IRolesService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public RolesService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Role?> GetRoleInCampaign(Guid? campaignGuid, int? userId)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userId
        });
        
        var res = await _dbAccess.GetData<Role, DynamicParameters>(StoredProcedureNames.GetUserRoleInCampaign, param);
        
        return res.FirstOrDefault();
    }
    
    public async Task<IEnumerable<Role>> GetRolesInCampaign(Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        
        var res = await _dbAccess.GetData<Role, DynamicParameters>(StoredProcedureNames.GetCampaignRoles, param);
        
        return res;
    }

    public async Task<int> AddRoleToCampaign(Guid? campaignGuid, string? roleName, string? roleDescription)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            roleName,
            roleDescription
        });
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddCustomRole, param);
        return param.Get<int>("returnVal");
    }
}