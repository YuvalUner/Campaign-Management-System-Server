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
    /// <param name="filterOptions"></param>
    /// <returns></returns>
    Task<IEnumerable<VoterLedgerFilterRecord>> GetFilteredVotersLedgerResults(VotersLedgerFilter filterOptions);
}