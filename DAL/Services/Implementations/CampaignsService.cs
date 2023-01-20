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
            campaignCreatorUserId,
            campaign.CityName
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
        param.Add("@IsUserInCampaign", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.IsUserInCampaign, param);
        return Convert.ToBoolean(param.Get<int>("@IsUserInCampaign"));
    }
    
    public async Task<CampaignType> GetCampaignType(Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        var res = await _dbAccess.GetData<CampaignType, DynamicParameters>
            (StoredProcedureNames.GetCampaignType, param);
        return res.FirstOrDefault();
    }
    
    public async Task<IEnumerable<User>> GetUsersInCampaign(Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        return await _dbAccess.GetData<User, DynamicParameters>
            (StoredProcedureNames.GetUsersInCampaign, param);
    }

    public async Task DeleteCampaign(Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteCampaign, param);
    }
}