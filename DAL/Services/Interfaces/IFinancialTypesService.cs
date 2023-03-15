using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for storing, retrieving, and modifying data related to financial types.
/// </summary>
public interface IFinancialTypesService
{
    /// <summary>
    /// Adds a new financial type to a campaign.
    /// </summary>
    /// <param name="financialType">An instance of <see cref="FinancialType"/> with all the required fields filled in.</param>
    /// <returns>Item 1: Status code <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist,
    /// <see cref="CustomStatusCode.TooManyEntries"/> if the campaign already has 100 financial types.<br/>
    /// Item 2: Guid of the newly created financial type. Guid.<see cref="Guid.Empty"/> in case of failure.</returns>
    Task<(CustomStatusCode, Guid)> CreateFinancialType(FinancialType financialType);

    /// <summary>
    /// Updates an existing financial type.
    /// </summary>
    /// <param name="financialType">An instance of <see cref="FinancialType"/> with the guid filled in, as well as any
    /// field that should be updated.</param>
    /// <returns>Status code <see cref="CustomStatusCode.FinancialTypeNotFound"/> if the financial type does not exist.</returns>
    Task<CustomStatusCode> UpdateFinancialType(FinancialType financialType);
    
    /// <summary>
    /// Deletes an existing financial type.
    /// </summary>
    /// <param name="typeGuid">Guid of the type to delete.</param>
    /// <returns>Status code <see cref="CustomStatusCode.FinancialTypeNotFound"/> if the financial type does not exist.</returns>
    Task<CustomStatusCode> DeleteFinancialType(Guid typeGuid);

    /// <summary>
    /// Gets all financial types for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>An enumerable of <see cref="FinancialType"/>, containing all the required info about each type.</returns>
    Task<IEnumerable<FinancialType>> GetFinancialTypes(Guid campaignGuid);
}