using DAL.Models;

namespace DAL.Services.Interfaces;

public interface ICitiesService
{
    Task<IEnumerable<City>> GetAllCities();
}