using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IVotersLedgerService
{
    /// <summary>
    /// Gets a single record from the voters ledger table by the given id number.
    /// </summary>
    /// <param name="voterId"></param>
    /// <returns></returns>
    Task<VotersLedgerRecord?> GetSingleVotersLedgerRecord(int? voterId);
    
    /// <summary>
    /// Gets results from the voters ledger table by the given search criteria.
    /// The results also include the user's support status for a campaign and their assigned ballot.
    /// </summary>
    /// <param name="filterOptions">An object containing the list of which filters to use and their values</param>
    /// <returns></returns>
    Task<IEnumerable<VoterLedgerFilterRecord>> GetFilteredVotersLedgerResults(VotersLedgerFilter filterOptions);

    /// <summary>
    /// Updates the support status of a voter in the voters ledger table.
    /// </summary>
    /// <param name="updateParams">An object containing the voter's id number and support status</param>
    /// <param name="campaignGuid"></param>
    /// <returns>The success or error code from the database</returns>
    Task<CustomStatusCode> UpdateVoterSupportStatus(UpdateSupportStatusParams updateParams, Guid campaignGuid);
}