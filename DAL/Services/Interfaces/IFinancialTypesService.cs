using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IFinancialTypesService
{
    /// <summary>
    /// Adds a new financial type to a campaign.
    /// </summary>
    /// <param name="financialType"></param>
    /// <returns>Status code CampaignNotFound if the campaign does not exist, TooManyEntries if the campaign already has
    /// 100 financial types.</returns>
    Task<(CustomStatusCode, Guid)> CreateFinancialType(FinancialType financialType);

    /// <summary>
    /// Updates an existing financial type.
    /// </summary>
    /// <param name="financialType"></param>
    /// <returns>Status code FinancialTypeNotFound if the financial type does not exist.</returns>
    Task<CustomStatusCode> UpdateFinancialType(FinancialType financialType);
    
    /// <summary>
    /// Deletes an existing financial type.
    /// </summary>
    /// <param name="typeGuid"></param>
    /// <returns>Status code FinancialTypeNotFound if the financial type does not exist.</returns>
    Task<CustomStatusCode> DeleteFinancialType(Guid typeGuid);

    /// <summary>
    /// Gets all financial types for a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<FinancialType>> GetFinancialTypes(Guid campaignGuid);
}