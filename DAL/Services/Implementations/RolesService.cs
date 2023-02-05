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

    public async Task<CustomStatusCode> AddRoleToCampaign(Guid? campaignGuid, string? roleName, string? roleDescription)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            roleName,
            roleDescription
        });
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddCustomRole, param);
        return (CustomStatusCode) param.Get<int>("returnVal");
    }
    
    public async Task DeleteRole(Guid? campaignGuid, string? roleName)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
        });
        // This parameter is added this way as it may be in Hebrew, and doing it this way guarantees treatment as nvarchar
        param.Add("roleName", dbType: DbType.String, direction: ParameterDirection.Input, value: roleName);
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteRole, param);
    }

    public async Task<CustomStatusCode> AssignUserToNormalRole(Guid? campaignGuid, string? userEmail, string? roleName)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userEmail,
        });
        param.Add("roleName", dbType: DbType.String, direction: ParameterDirection.Input, value: roleName);
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AssignUserToRole, param);
        return (CustomStatusCode) param.Get<int>("returnVal");
    }
    
    public async Task<CustomStatusCode> AssignUserToAdministrativeRole(Guid? campaignGuid, string? userEmail, string? roleName)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userEmail,
        });
        param.Add("roleName", dbType: DbType.String, direction: ParameterDirection.Input, value: roleName);
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AssignUserToAdministrativeRole, param);
        return (CustomStatusCode) param.Get<int>("returnVal");
    }
    
    public async Task UpdateRole(Guid? campaignGuid, string? roleName, string? roleDescription)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            roleDescription
        });
        param.Add("roleName", dbType: DbType.String, direction: ParameterDirection.Input, value: roleName);
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateRole, param);
    }
    
    public async Task RemoveUserFromRole(Guid? campaignGuid, string? userEmail)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userEmail
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveUserFromRole, param);
    }
    
    public async Task RemoveUserFromAdministrativeRole(Guid? campaignGuid, string? userEmail)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userEmail
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveUserFromAdministrativeRole, param);
    }

    public async Task<Role?> GetRole(string? roleName, Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        param.Add("roleName", dbType: DbType.String, direction: ParameterDirection.Input, value: roleName);
        var res = await _dbAccess.GetData<Role, DynamicParameters>(StoredProcedureNames.GetRole, param);
        return res.FirstOrDefault();
    }
}