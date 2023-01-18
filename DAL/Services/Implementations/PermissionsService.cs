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

    public async Task AddPermission(Permission permission, int? userId, Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            permission.PermissionType,
            permission.PermissionForScreen,
            userId,
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.AddPermission, param);
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
}