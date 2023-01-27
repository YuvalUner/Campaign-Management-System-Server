﻿using System.Data;
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

    public async Task<StatusCodes> AddRoleToCampaign(Guid? campaignGuid, string? roleName, string? roleDescription)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            roleName,
            roleDescription
        });
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddCustomRole, param);
        return (StatusCodes) param.Get<int>("returnVal");
    }
    
    public async Task DeleteRole(Guid? campaignGuid, string? roleName)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            roleName
        });
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteRole, param);
    }

    public async Task<StatusCodes> AssignUserToNormalRole(Guid? campaignGuid, string? userEmail, string? roleName)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userEmail,
            roleName
        });
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AssignUserToRole, param);
        return (StatusCodes) param.Get<int>("returnVal");
    }
    
    public async Task<StatusCodes> AssignUserToAdministrativeRole(Guid? campaignGuid, string? userEmail, string? roleName)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userEmail,
            roleName
        });
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AssignUserToAdministrativeRole, param);
        return (StatusCodes) param.Get<int>("returnVal");
    }
    
    public async Task UpdateRole(Guid? campaignGuid, string? roleName, string? roleDescription)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            roleName,
            roleDescription
        });
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
            roleName,
            campaignGuid
        });
        var res = await _dbAccess.GetData<Role, DynamicParameters>(StoredProcedureNames.GetRole, param);
        return res.FirstOrDefault();
    }
}