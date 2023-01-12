using DAL.DbAccess;

namespace DAL.Services;

public class CampaignsService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public CampaignsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    
}