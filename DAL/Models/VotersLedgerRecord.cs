namespace DAL.Models;

public class VotersLedgerRecord
{
    public int IdNum { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FathersName { get; set; }
    public int? CityId { get; set; }
    public int? BallotId { get; set; }
    public string? Spare1 { get; set; }
    public int? ResidenceId { get; set; }
    public string? ResidenceName { get; set; }
    public string? Spare2 { get; set; }
    public int? StreetId { get; set; }
    public string? StreetName { get; set; }
    public int? HouseNumber { get; set; }
    public string? Entrance { get; set; }
    public int? Appartment { get; set; }
    public string? HouseLetter { get; set; }
    public int? BallotSerial { get; set; }
    public string? Spare3 { get; set; }
    public string? Spare4 { get; set; }
    public int? ZipCode { get; set; }
    public string? Spare5 { get; set; }
    public int? CampaignYear { get; set; }
}