using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class FinancialDataService: IFinancialDataService
{
    private readonly IGenericDbAccess _genericDbAccess;

    public FinancialDataService(IGenericDbAccess genericDbAccess)
    {
        _genericDbAccess = genericDbAccess;
    }

    public async Task<(CustomStatusCode, Guid)> AddFinancialDataEntry(FinancialDataEntry dataEntry)
    {
        var param = new DynamicParameters(new
        {
            dataEntry.Amount,
            dataEntry.CampaignGuid,
            dataEntry.CreatorUserId,
            dataEntry.DataDescription,
            dataEntry.TypeGuid,
            dataEntry.DataTitle,
            dataEntry.IsExpense,
            dataEntry.DateCreated,
        });

        param.Add("newDataGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _genericDbAccess.ModifyData(StoredProcedureNames.AddFinancialData, param);

        return (param.Get<CustomStatusCode>("returnVal"), param.Get<Guid>("newDataGuid"));
    }

    public async Task<CustomStatusCode> UpdateFinancialDataEntry(FinancialDataEntry dataEntry)
    {
        var param = new DynamicParameters(new
        {
            dataEntry.Amount,
            dataEntry.DataDescription,
            dataEntry.TypeGuid,
            dataEntry.DataTitle,
            dataEntry.IsExpense,
            dataEntry.DateCreated,
            dataEntry.DataGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _genericDbAccess.ModifyData(StoredProcedureNames.UpdateFinancialData, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> DeleteFinancialDataEntry(Guid dataGuid)
    {
        var param = new DynamicParameters(new
        {
            dataGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _genericDbAccess.ModifyData(StoredProcedureNames.DeleteFinancialData, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<IEnumerable<FinancialSummaryEntry>> GetFinancialSummary(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });

        return await _genericDbAccess.GetData<FinancialSummaryEntry, DynamicParameters>(
            StoredProcedureNames.GetFinancialSummaryForCampaign, param);
    }
    
    public async Task<IEnumerable<FinancialDataEntryWithTypeAndCreator>> GetFinancialDataForCampaign(Guid campaignGuid,
        Guid? typeGuid = null)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            typeGuid
        });

        return await _genericDbAccess.GetData<FinancialDataEntryWithTypeAndCreator, DynamicParameters>(
            StoredProcedureNames.GetFinancialDataForCampaign, param);
    }
}