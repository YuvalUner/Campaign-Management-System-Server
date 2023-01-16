namespace DAL.Models;

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