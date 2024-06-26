﻿using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class PermissionsService : IPermissionsService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public PermissionsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<CustomStatusCode> AddPermission(Permission permission, int? userId, Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            permission.PermissionType,
            permission.PermissionTarget,
            userId,
            campaignGuid
        });
        param.Add("returnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddPermission, param);
        return (CustomStatusCode)param.Get<int>("returnCode");
    }
    
    public async Task<IEnumerable<Permission?>> GetPermissions(int? userId, Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid
        });
        return await _dbAccess.GetData<Permission, DynamicParameters>(StoredProcedureNames.GetPermissions, param);
    }
    
    public async Task RemovePermission(Permission permission, int? userId, Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid,
            permission.PermissionType,
            permission.PermissionTarget
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RemovePermission, param);
    }
}