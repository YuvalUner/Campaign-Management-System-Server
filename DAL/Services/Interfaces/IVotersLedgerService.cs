using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for getting voters ledger records, as well as modifying some details in them.
/// </summary>
public interface IVotersLedgerService
{
    /// <summary>
    /// Gets a single record from the voters ledger table by the given id number.
    /// </summary>
    /// <param name="voterId">The Id number of a single voter.</param>
    /// <returns>A single <see cref="VotersLedgerRecord"/> object with all of its fields populated (assuming they are not
    /// null in the database) if person with that id number exists.</returns>
    Task<VotersLedgerRecord?> GetSingleVotersLedgerRecord(int? voterId);

    /// <summary>
    /// Gets results from the voters ledger table by the given search criteria.
    /// The results also include the user's support status for a campaign and their assigned ballot.
    /// </summary>
    /// <param name="filterOptions">A <see cref="VotersLedgerFilter"/> object containing which filters to use and their values</param>
    /// <returns>A list of <see cref="VotersLedgerFilterRecord"/>, with each containing a record about a user that
    /// matched the filters.</returns>
    Task<IEnumerable<VotersLedgerFilterRecord>> GetFilteredVotersLedgerResults(VotersLedgerFilter filterOptions);

    /// <summary>
    /// Updates the support status of a voter in the voters ledger table.
    /// </summary>
    /// <param name="updateParams">An object containing the voter's id number and support status</param>
    /// <param name="campaignGuid">Guid of the campaign making the request.</param>
    /// <returns><see cref="CustomStatusCode.CityNotFound"/> error code if it could not find a connection between the given ID
    /// number and the city the campaign is in.</returns>
    Task<CustomStatusCode> UpdateVoterSupportStatus(UpdateSupportStatusParams updateParams, Guid campaignGuid);
}