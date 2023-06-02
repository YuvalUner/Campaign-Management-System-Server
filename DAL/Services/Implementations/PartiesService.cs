using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;


public class PartiesService : IPartiesService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public PartiesService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<CustomStatusCode> AddParty(Party party, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            party.PartyLetter,
            party.PartyName,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AddParty, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> UpdateParty(Party party, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            party.PartyId,
            party.PartyLetter,
            party.PartyName,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateParty, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> DeleteParty(int partyId, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            partyId,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteParty, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<IEnumerable<Party>> GetParties(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        
        return await _dbAccess.GetData<Party, DynamicParameters>(StoredProcedureNames.GetPartiesForCampaign, param);
    }
}