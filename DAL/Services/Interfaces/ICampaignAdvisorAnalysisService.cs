using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface ICampaignAdvisorAnalysisService
{
    Task<(CustomStatusCode, Guid)> AddAnalysisOverview(AnalysisOverview overview, Guid campaignGuid);
    Task AddAnalysisDetailsRow(AnalysisRow row);
    Task AddAnalysisSample(AnalysisSample sample);

    Task AddAnalysisDetailsRows(List<AnalysisRow> rows);
    
    Task AddAnalysisSamples(List<AnalysisSample> samples);
}