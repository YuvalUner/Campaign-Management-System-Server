using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IFinancialDataService
{
    /// <summary>
    /// Adds a new financial data entry for a campaign to the database.
    /// </summary>
    /// <param name="dataEntry"></param>
    /// <returns>Status code CampaignNotFound if the campaign does not exist, FinancialTypeNotFound if the financial type
    /// does not exist, UserNotFound if the user does not exist.</returns>
    Task<(CustomStatusCode, Guid)> AddFinancialDataEntry(FinancialDataEntry dataEntry);
    
    /// <summary>
    /// Updates an existing financial data entry for a campaign in the database.
    /// </summary>
    /// <param name="dataEntry"></param>
    /// <returns>Status code FinancialDataNotFound if the financial data does not exist, FinancialTypeNotFound if the
    ///  financial type does not exist.</returns>
    Task<CustomStatusCode> UpdateFinancialDataEntry(FinancialDataEntry dataEntry);
    
    /// <summary>
    /// Deletes an existing financial data entry for a campaign from the database.
    /// </summary>
    /// <param name="dataGuid"></param>
    /// <returns>Status code FinancialDataNotFound if the financial data does not exist.</returns>
    Task<CustomStatusCode> DeleteFinancialDataEntry(Guid dataGuid);
    
    /// <summary>
    /// Gets a summary of the financial data for a campaign - total income and total expense, grouped by financial type.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<FinancialSummaryEntry>> GetFinancialSummary(Guid campaignGuid);

    /// <summary>
    /// Gets all financial data entries for a campaign. If a financial type is specified, only entries of that type are
    /// returned.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="typeGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<FinancialDataEntryWithTypeAndCreator>> GetFinancialDataForCampaign(Guid campaignGuid, Guid? typeGuid = null);
}