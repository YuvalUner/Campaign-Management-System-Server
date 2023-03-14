namespace DAL.Models;

/// <summary>
/// A model for the cities table.<br/>
/// Only contains the city id and the city name.<br/>
/// City id is a value determined by official sources (government data), and is not auto-incremented.<br/>
/// </summary>
public class City
{
    public int? CityId { get; set; }
    public string? CityName { get; set; }
}