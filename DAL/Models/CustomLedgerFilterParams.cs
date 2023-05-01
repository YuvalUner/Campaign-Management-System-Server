namespace DAL.Models;

public class CustomLedgerFilterParams
{
    public Guid LedgerGuid { get; set; }
    public int? IdNum { get; set; }
    public string? CityName { get; set; }
    public string? StreetName { get; set; }
    public Decimal? BallotId { get; set; }
    public bool? SupportStatus { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}