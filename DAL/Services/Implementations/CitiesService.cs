using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;

namespace DAL.Services.Implementations;

public class CitiesService : ICitiesService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public CitiesService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    public async Task<IEnumerable<City>> GetAllCities()
    {
        return await _dbAccess.GetData<City, string?>(StoredProcedureNames.GetAllCities, null);
    }
}