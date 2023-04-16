using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class CustomVotersLedgerService: ICustomVotersLedgerService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public CustomVotersLedgerService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    public async Task<(CustomStatusCode, Guid)> CreateCustomVotersLedger(CustomVotersLedger customVotersLedger)
    {
        var param = new DynamicParameters(new
        {
            customVotersLedger.LedgerName,
            customVotersLedger.CampaignGuid
        });
        
        param.Add("newLedgerGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AddCustomVotersLedger, param);
        
        return (param.Get<CustomStatusCode>("returnVal"), param.Get<Guid>("newLedgerGuid"));
    }
    
    public async Task<CustomStatusCode> DeleteCustomVotersLedger(Guid ledgerGuid, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            ledgerGuid,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteCustomVotersLedger, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> UpdateCustomVotersLedger(CustomVotersLedger customVotersLedger, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            customVotersLedger.LedgerGuid,
            newLedgerName = customVotersLedger.LedgerName,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateCustomVotersLedger, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }

    public Task<IEnumerable<CustomVotersLedger>> GetCustomVotersLedgersByCampaignGuid(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        
        return _dbAccess.GetData<CustomVotersLedger,
            DynamicParameters>(StoredProcedureNames.GetCampaignCustomVotersLedgers, param);
    }
}