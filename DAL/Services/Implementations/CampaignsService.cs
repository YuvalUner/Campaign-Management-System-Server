using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class CampaignsService : ICampaignsService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public CampaignsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<int> AddCampaign(Campaign campaign, int? campaignCreatorUserId)
    {
        var param = new DynamicParameters(new
        {
            campaign.CampaignName,
            campaign.CampaignDescription,
            campaign.IsMunicipal,
            campaign.CampaignLogoUrl,
            campaignCreatorUserId
        });
        param.Add("@CampaignId", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddCampaign, param);
        return param.Get<int>("@CampaignId");
    }
    
    public async Task ModifyCampaign(Campaign campaign)
    {
        var param = new DynamicParameters(new
        {
            campaign.CampaignGuid,
            campaign.CampaignDescription,
            campaign.CampaignLogoUrl
        });
        await _dbAccess.ModifyData(StoredProcedureNames.ModifyCampaign, param);
    }
    
    public async Task<Guid?> GetCampaignGuid(int? campaignId)
    {
        var param = new DynamicParameters(new
        {
            campaignId
        });
        var res = await _dbAccess.GetData<Campaign, DynamicParameters>(StoredProcedureNames.GetGuidByCampaignId, param);
        return res.FirstOrDefault()?.CampaignGuid;
    }

    public async Task<Guid?> GetCampaignGuidByInviteGuid(Guid? campaignInviteGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignInviteGuid
        });
        var res = await _dbAccess.GetData<Campaign, DynamicParameters>
            (StoredProcedureNames.GetCampaignGuidByInviteGuid, param);
        return res.FirstOrDefault()?.CampaignGuid;
    }

    public async Task<bool> IsUserInCampaign(Guid? campaignGuid, int? userId)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            userId
        });
        param.Add("@IsUserInCampaign", dbType: DbType.Boolean, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.IsUserInCampaign, param);
        return param.Get<bool>("@IsUserInCampaign");
    }
}