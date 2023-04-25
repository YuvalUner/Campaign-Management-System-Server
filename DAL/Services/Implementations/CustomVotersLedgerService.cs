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

    public async Task<CustomStatusCode> AddCustomVotersLedgerRow(CustomVotersLedgerContent customVotersLedgerContent, 
        Guid ledgerGuid)
    {
        var param = new DynamicParameters(new
        {
            customVotersLedgerContent.Identifier,
            ledgerGuid,
            customVotersLedgerContent.FirstName,
            customVotersLedgerContent.LastName,
            customVotersLedgerContent.CityName,
            customVotersLedgerContent.BallotId,
            customVotersLedgerContent.StreetName,
            customVotersLedgerContent.HouseNumber,
            customVotersLedgerContent.ZipCode,
            customVotersLedgerContent.Entrance,
            customVotersLedgerContent.Appartment,
            customVotersLedgerContent.HouseLetter,
            customVotersLedgerContent.Email1,
            customVotersLedgerContent.Email2,
            customVotersLedgerContent.Phone1,
            customVotersLedgerContent.Phone2,
            customVotersLedgerContent.SupportStatus
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AddCustomVotersLedgerRow, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> DeleteCustomVotersLedgerRow(Guid ledgerGuid, int rowId)
    {
        var param = new DynamicParameters(new
        {
            ledgerGuid,
            identifier = rowId
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteCustomVotersLedgerRow, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> UpdateCustomVotersLedgerRow(CustomVotersLedgerContent customVotersLedgerContent, 
        Guid ledgerGuid)
    {
        var param = new DynamicParameters(new
        {
            customVotersLedgerContent.Identifier,
            ledgerGuid,
            customVotersLedgerContent.FirstName,
            customVotersLedgerContent.LastName,
            customVotersLedgerContent.CityName,
            customVotersLedgerContent.BallotId,
            customVotersLedgerContent.StreetName,
            customVotersLedgerContent.HouseNumber,
            customVotersLedgerContent.ZipCode,
            customVotersLedgerContent.Entrance,
            customVotersLedgerContent.Appartment,
            customVotersLedgerContent.HouseLetter,
            customVotersLedgerContent.Email1,
            customVotersLedgerContent.Email2,
            customVotersLedgerContent.Phone1,
            customVotersLedgerContent.Phone2,
            customVotersLedgerContent.SupportStatus
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateCustomVotersLedgerRow, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<IEnumerable<CustomVotersLedgerContent>> FilterCustomVotersLedger(Guid ledgerGuid, 
        CustomLedgerFilterParams filter)
    {
        return await _dbAccess.GetData<CustomVotersLedgerContent, CustomLedgerFilterParams>
            (StoredProcedureNames.FilterCustomVotersLedger, filter);
    }
    

    public async Task<CustomStatusCode> ImportLedger(Guid ledgerGuid, string jsonLedger)
    {
        var param = new DynamicParameters(new
        {
            ledgerGuid,
            jsonLedger
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.ImportLedger, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
}