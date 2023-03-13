namespace DAL.Models;

/// <summary>
/// A model used for filtering the voters ledger table.<br/>
/// All parameters but CampaignGuid are optional, and if a parameter is null, it will not be used for filtering.
/// </summary>
public class VotersLedgerFilter
{
    public Guid? CampaignGuid { get; set; }
    public int? IdNum { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Decimal? BallotId { get; set; }
    public string? CityName { get; set; }
    public string? StreetName { get; set; }
    public bool? SupportStatus { get; set; }
}