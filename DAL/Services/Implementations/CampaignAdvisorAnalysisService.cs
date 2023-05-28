using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class CampaignAdvisorAnalysisService : ICampaignAdvisorAnalysisService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public CampaignAdvisorAnalysisService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<(CustomStatusCode, Guid)> AddAnalysisOverview(AnalysisOverview overview, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid,
            overview.AnalysisTarget,
            overview.ResultsTitle,
            overview.TargetTwitterHandle,
            overview.MaxDaysBack,
            overview.AdditionalUserRequests,
        });
        
        param.Add("newResultsGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AddAnalysisOverview, param);
        
        return (param.Get<CustomStatusCode>("returnVal"), param.Get<Guid>("newResultsGuid"));
    }

    public async Task AddAnalysisDetailsRow(AnalysisRow row)
    {
        await _dbAccess.ModifyData(StoredProcedureNames.AddAnalysisDetails, row);
    }

    public async Task AddAnalysisDetailsRows(List<AnalysisRow> rows)
    {
        foreach (var row in rows)
        {
            await AddAnalysisDetailsRow(row);
        }
    }
    
    public async Task AddAnalysisSample(AnalysisSample sample)
    {
        await _dbAccess.ModifyData(StoredProcedureNames.AddAnalysisSample, sample);
    }
    
    public async Task AddAnalysisSamples(List<AnalysisSample> samples)
    {
        foreach (var sample in samples)
        {
            await AddAnalysisSample(sample);
        }
    }
}