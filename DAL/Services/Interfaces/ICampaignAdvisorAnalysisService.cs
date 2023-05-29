using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface ICampaignAdvisorAnalysisService
{
    /// <summary>
    /// Adds a new analysis overview to the database. Should be done before adding any details rows or samples.
    /// </summary>
    /// <param name="overview"></param>
    /// <param name="campaignGuid"></param>
    /// <returns>Item 1: CampaignNotFound or Ok. Item 2: Guid of the newly created overview.</returns>
    Task<(CustomStatusCode, Guid)> AddAnalysisOverview(AnalysisOverview overview, Guid campaignGuid);
    
    /// <summary>
    /// Adds a single analysis details row to the database.
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    Task AddAnalysisDetailsRow(AnalysisRow row);
    
    /// <summary>
    /// Adds a single analysis sample to the database.
    /// </summary>
    /// <param name="sample"></param>
    /// <returns></returns>
    Task AddAnalysisSample(AnalysisSample sample);

    /// <summary>
    /// Adds a list of analysis details rows to the database.
    /// </summary>
    /// <param name="rows"></param>
    /// <returns></returns>
    Task AddAnalysisDetailsRows(List<AnalysisRow> rows);
    
    /// <summary>
    /// Adds a list of analysis samples to the database.
    /// </summary>
    /// <param name="samples"></param>
    /// <returns></returns>
    Task AddAnalysisSamples(List<AnalysisSample> samples);

    /// <summary>
    /// Gets the analysis overview for a given results guid.
    /// </summary>
    /// <param name="resultsGuid"></param>
    /// <returns></returns>
    Task<AnalysisOverview?> GetAnalysisOverview(Guid resultsGuid);

    /// <summary>
    /// Gets all the analysis details rows for a given results guid.
    /// </summary>
    /// <param name="resultsGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<AnalysisRow>> GetAnalysisDetails(Guid resultsGuid);

    /// <summary>
    /// Gets all the analysis samples for a given results guid.
    /// </summary>
    /// <param name="resultsGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<AnalysisSample>> GetAnalysisSamples(Guid resultsGuid);

    /// <summary>
    /// Gets an <see cref="AdvisorResults"/> object populated with all the analysis data for a given results guid,
    /// including its overview, details and samples.
    /// </summary>
    /// <param name="resultsGuid"></param>
    /// <returns></returns>
    Task<AdvisorResults> GetAdvisorResults(Guid resultsGuid);

    /// <summary>
    /// Gets the title, target name, result guid and time performed for all the analyses performed for a given campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<AnalysisOverview>> GetAnalysisOverviewForCampaign(Guid campaignGuid);

    /// <summary>
    /// Updates or adds a GPT response to the analysis overview for a given results guid.
    /// </summary>
    /// <param name="resultsGuid"></param>
    /// <param name="gptResponse"></param>
    /// <returns></returns>
    Task UpdateAnalysisGptResponse(Guid resultsGuid, string gptResponse);

    /// <summary>
    /// Deletes all analysis data for a given results guid.
    /// </summary>
    /// <param name="resultsGuid"></param>
    /// <returns></returns>
    Task DeleteAnalysis(Guid resultsGuid);
}