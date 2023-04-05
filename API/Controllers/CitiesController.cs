using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class CitiesController: Controller
{
    private readonly ICitiesService _citiesService;
    private readonly ILogger<CitiesController> _logger;
    
    public CitiesController(ICitiesService citiesService, ILogger<CitiesController> logger)
    {
        _citiesService = citiesService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCities()
    {
        try
        {
            var cities = await _citiesService.GetAllCities();
            return Ok(cities);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to get all cities");
            return StatusCode(500, "Error while trying to get all cities");
        }
    }
}