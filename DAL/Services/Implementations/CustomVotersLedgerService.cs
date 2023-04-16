using DAL.DbAccess;
using DAL.Services.Interfaces;

namespace DAL.Services.Implementations;

public class CustomVotersLedgerService: ICustomVotersLedgerService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public CustomVotersLedgerService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
}