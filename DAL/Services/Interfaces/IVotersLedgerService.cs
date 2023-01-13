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
}