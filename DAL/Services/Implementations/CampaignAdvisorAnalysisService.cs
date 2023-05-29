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
    
    public async Task<AnalysisOverview?> GetAnalysisOverview(Guid resultsGuid)
    {
        var param = new DynamicParameters(new
        {
            resultsGuid
        });
        
        var res =  await _dbAccess.GetData<AnalysisOverview, DynamicParameters>(StoredProcedureNames.GetAnalysisOverview, param);
        return res.FirstOrDefault();
    }
    
    public async Task<IEnumerable<AnalysisRow>> GetAnalysisDetails(Guid resultsGuid)
    {
        var param = new DynamicParameters(new
        {
            resultsGuid
        });
        
        return await _dbAccess.GetData<AnalysisRow, DynamicParameters>(StoredProcedureNames.GetAnalysisDetails, param);
    }
    
    public async Task<IEnumerable<AnalysisSample>> GetAnalysisSamples(Guid resultsGuid)
    {
        var param = new DynamicParameters(new
        {
            resultsGuid
        });
        
        return await _dbAccess.GetData<AnalysisSample, DynamicParameters>(StoredProcedureNames.GetAnalysisSamples, param);
    }
    
    public async Task<AdvisorResults> GetAdvisorResults(Guid resultsGuid)
    {
        var results = new AdvisorResults();
        
        results.Overview = await GetAnalysisOverview(resultsGuid);
        results.Details = await GetAnalysisDetails(resultsGuid);
        results.Samples = await GetAnalysisSamples(resultsGuid);
        
        return results;
    }
    
    public async Task<IEnumerable<AnalysisOverview>> GetAnalysisOverviewForCampaign(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        
        return await _dbAccess.GetData<AnalysisOverview, DynamicParameters>(StoredProcedureNames.GetAnalysisOverviewForCampaign, param);
    }

    public async Task UpdateAnalysisGptResponse(Guid resultsGuid, string gptResponse)
    {
        var param = new DynamicParameters(new
        {
            resultsGuid,
            gptResponse
        });
        
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateAnalysisGptResponse, param);
    }

    public async Task DeleteAnalysis(Guid resultsGuid)
    {
        var param = new DynamicParameters(new
        {
            resultsGuid
        });
        
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteAnalysis, param);
    }
}