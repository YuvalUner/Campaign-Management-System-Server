using System.Security.Claims;
using DAL.DbAccess;
using DAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IGenericDbAccess _dbAccess;
    private readonly IConfiguration _configuration;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IGenericDbAccess dbAccess, IConfiguration config)
    {
        _logger = logger;
        _dbAccess = dbAccess;
        _configuration = config;
    }

    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        HttpContext.Session.SetString("Name", "The Doctor");
        HttpContext.Session.SetInt32("Age", 73);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "aaa"),
            new Claim("FullName", "bbb"),
            new Claim(ClaimTypes.Role, "Administrator"),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        var authProperties = new AuthenticationProperties
        {
            //AllowRefresh = <bool>,
            // Refreshing the authentication session should be allowed.

            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            // The time at which the authentication ticket expires. A 
            // value set here overrides the ExpireTimeSpan option of 
            // CookieAuthenticationOptions set with AddCookie.

            //IsPersistent = true,
            // Whether the authentication session is persisted across 
            // multiple requests. When used with cookies, controls
            // whether the cookie's lifetime is absolute (matching the
            // lifetime of the authentication ticket) or session-based.

            //IssuedUtc = <DateTimeOffset>,
            // The time at which the authentication ticket was issued.

            //RedirectUri = <string>
            // The full path or absolute URI to be used as an http 
            // redirect response value.
        };
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties);
        return Ok();
    }
    
    [HttpGet("Test2")]
    [Authorize]
    public IActionResult Test2()
    {
        var name = HttpContext.Session.GetString("Name");
        var age = HttpContext.Session.GetInt32("Age");
        return Ok(new {name, age});
    }

    [HttpGet("GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogCritical("GetWeatherForecast");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
    [HttpGet("TestDbConnection")]
    public async Task<IActionResult> TestDbConnection()
    {
        var res = await _dbAccess.GetData<City, DBNull>(StoredProcedureNames.TestConnection,
            DBNull.Value);
        return Ok(res);
    }
}