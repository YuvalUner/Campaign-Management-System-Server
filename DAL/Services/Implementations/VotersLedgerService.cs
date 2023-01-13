using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class VotersLedgerService : IVotersLedgerService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public VotersLedgerService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    public async Task<VotersLedgerRecord?> GetSingleVotersLedgerRecord(int? voterId)
    {
        var param = new DynamicParameters(new
        {
            voterId
        });
        var votersLedgerRecord = 
            await _dbAccess.GetData<VotersLedgerRecord, DynamicParameters>(StoredProcedureNames.GetFromVotersLedgerById, param);
        return votersLedgerRecord.FirstOrDefault();
    }
}