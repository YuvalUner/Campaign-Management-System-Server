using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface ICustomVotersLedgerService
{
    /// <summary>
    /// Creates a new custom voters ledger entity in the database, to prepare for adding data to it.
    /// </summary>
    /// <param name="customVotersLedger">An instance of <see cref="CustomVotersLedger"/> with ledgerName
    /// and campaignGuid.</param>
    /// <returns>Item 1: Status code CampaignNotFound if the Guid could not be associated to any campaign.<br/>
    /// Item 2: Guid of the newly created ledger (Guid.Empty if creation failed).</returns>
    Task<(CustomStatusCode, Guid)> CreateCustomVotersLedger(CustomVotersLedger customVotersLedger);

    /// <summary>
    /// Deletes a custom voters ledger entity from the database.
    /// </summary>
    /// <param name="ledgerGuid">Guid of the ledger to delete.</param>
    /// <returns>Status code LedgerNotFound if the ledger does not exist.</returns>
    Task<CustomStatusCode> DeleteCustomVotersLedger(Guid ledgerGuid);

    /// <summary>
    /// Updates an existing custom voters ledger entity in the database, to change its name.
    /// </summary>
    /// <param name="customVotersLedger">An instance of <see cref="CustomVotersLedger"/> with the ledgerName and
    /// ledgerGuid.</param>
    /// <returns>Status code LedgerNotFound if the ledger does not exist.</returns>
    Task<CustomStatusCode> EditCustomVotersLedger(CustomVotersLedger customVotersLedger);
    
    /// <summary>
    /// Gets the list of custom voters ledgers associated to a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get the list for.</param>
    /// <returns></returns>
    Task<IEnumerable<CustomVotersLedger>> GetCustomVotersLedgersByCampaignGuid(Guid campaignGuid);
}