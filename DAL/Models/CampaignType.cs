namespace DAL.Models;

/// <summary>
/// A model for the type of a campaign.<br/>
/// Describes whether the campaign is municipal or not, and if it is not, what city it is in.<br/>
/// </summary>
public class CampaignType
{
    public bool IsMunicipal { get; set; }
    public string? CityName { get; set; }
}