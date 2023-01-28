using System.Data;
using DAL.DbAccess;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class InvitesService : IInvitesService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public InvitesService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task CreateInvite(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.CreateCampaignInvite, param);
    }
    
    public async Task RevokeInvite(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RevokeCampaignInvite, param);
    }
    
    public async Task<Guid?> GetInvite(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        var res = await _dbAccess.GetData<Guid?, DynamicParameters>(StoredProcedureNames.GetCampaignInviteGuid, param);
        return res.FirstOrDefault();
    }

    public async Task<CustomStatusCode> AcceptInvite(Guid? campaignGuid, int? userId)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userId
        });
        param.Add("returnCode", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.LinkUserToCampaign, param);
        return (CustomStatusCode) param.Get<int>("returnCode");
    }
}