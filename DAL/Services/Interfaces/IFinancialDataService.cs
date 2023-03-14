using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for storing, retrieving, and modifying data related to financial data.
/// </summary>
public interface IFinancialDataService
{
    /// <summary>
    /// Adds a new financial data entry for a campaign to the database.
    /// </summary>
    /// <param name="dataEntry">An instance of <see cref="FinancialDataEntry"/> with all the required fields filled in.</param>
    /// <returns>Item1: Status code <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist,
    /// <see cref="CustomStatusCode.FinancialTypeNotFound"/> if the financial type given
    /// does not exist, <see cref="CustomStatusCode.UserNotFound"/> if the creating user does not exist.<br/>
    /// Item 2: Guid of the newly created entry.</returns>
    Task<(CustomStatusCode, Guid)> AddFinancialDataEntry(FinancialDataEntry dataEntry);
    
    /// <summary>
    /// Updates an existing financial data entry for a campaign in the database.
    /// </summary>
    /// <param name="dataEntry">An instance of <see cref="FinancialDataEntry"/> with all the fields to be updated filled in,
    /// as well as the Guid.</param>
    /// <returns>Status code <see cref="CustomStatusCode.FinancialDataNotFound"/> if the financial data does not exist,
    /// <see cref="CustomStatusCode.FinancialTypeNotFound"/> if the financial type given does not exist.</returns>
    Task<CustomStatusCode> UpdateFinancialDataEntry(FinancialDataEntry dataEntry);
    
    /// <summary>
    /// Deletes an existing financial data entry for a campaign from the database.
    /// </summary>
    /// <param name="dataGuid">The guid of the entry to delete.</param>
    /// <returns>Status code <see cref="CustomStatusCode.FinancialDataNotFound"/> if the financial data does not exist.</returns>
    Task<CustomStatusCode> DeleteFinancialDataEntry(Guid dataGuid);
    
    /// <summary>
    /// Gets a summary of the financial data for a campaign - total income and total expense, grouped by financial type.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <returns>An enumerable of <see cref="FinancialSummaryEntry"/>, each row containing the total expenses or incomes
    /// or a single financial type.</returns>
    Task<IEnumerable<FinancialSummaryEntry>> GetFinancialSummary(Guid campaignGuid);

    /// <summary>
    /// Gets all financial data entries for a campaign. If a financial type is specified, only entries of that type are
    /// returned.
    /// </summary>
    /// <param name="campaignGuid">The guid of the campaign.</param>
    /// <param name="typeGuid">The guid of the type to filter by, optional.</param>
    /// <returns>A list of <see cref="FinancialDataEntryWithTypeAndCreator"/>, containing info about each entry.</returns>
    Task<IEnumerable<FinancialDataEntryWithTypeAndCreator>> GetFinancialDataForCampaign(Guid campaignGuid, Guid? typeGuid = null);
}