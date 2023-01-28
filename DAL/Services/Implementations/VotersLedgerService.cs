using System.Data;
using System.Text;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class VotersLedgerService : IVotersLedgerService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public VotersLedgerService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    public async Task<VotersLedgerRecord?> GetSingleVotersLedgerRecord(int? voterId)
    {
        var param = new DynamicParameters(new
        {
            voterId
        });
        var votersLedgerRecord = 
            await _dbAccess.GetData<VotersLedgerRecord, DynamicParameters>(StoredProcedureNames.GetFromVotersLedgerById, param);
        return votersLedgerRecord.FirstOrDefault();
    }

    public async Task<IEnumerable<VoterLedgerFilterRecord>> GetFilteredVotersLedgerResults(VotersLedgerFilter filterOptions)
    {
        var param = new DynamicParameters(new
        {
            idNum = filterOptions.IdNum,
            campaignGuid = filterOptions.CampaignGuid,
        });
        // Only adding in the parameters that are not null, so that the stored procedure can handle the nulls
        // They are added in this way because putting them in the initial dict causes issues with 
        // the database collation, as some of the columns have hebrew values to them.
        if (filterOptions.CityName != null)
        {
            param.Add("cityName", filterOptions.CityName, DbType.String, ParameterDirection.Input, 100); 
        }
        if (filterOptions.StreetName != null)
        {
            param.Add("streetName", filterOptions.StreetName, DbType.String, ParameterDirection.Input, 100); 
        }
        if (filterOptions.FirstName != null)
        {
            param.Add("firstName", filterOptions.FirstName, DbType.String, ParameterDirection.Input, 100); 
        }
        if (filterOptions.LastName != null)
        {
            param.Add("lastName", filterOptions.LastName, DbType.String, ParameterDirection.Input, 100); 
        }
        if (filterOptions.SupportStatus != null)
        {
            param.Add("supportStatus", filterOptions.SupportStatus, DbType.Boolean, ParameterDirection.Input); 
        }
        if (filterOptions.BallotId != null)
        {
            param.Add("ballotId", Decimal.Round(filterOptions.BallotId.Value, 1), DbType.Decimal, ParameterDirection.Input);
        }
        
        var res = await _dbAccess.GetData<VoterLedgerFilterRecord, DynamicParameters>
            (StoredProcedureNames.FilterVotersLedger, param);
        return res;
    }

    public async Task<CustomStatusCode> UpdateVoterSupportStatus(UpdateSupportStatusParams updateParams, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            voterId = updateParams.IdNum,
            campaignGuid
        });
        if (updateParams.SupportStatus != null)
        {
            param.Add("supportStatus", updateParams.SupportStatus, DbType.Boolean, ParameterDirection.Input); 
        }
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateSupportStatus, param);
        return (CustomStatusCode)param.Get<int>("returnVal");
    }
}