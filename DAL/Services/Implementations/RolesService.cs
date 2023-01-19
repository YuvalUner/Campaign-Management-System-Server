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
}