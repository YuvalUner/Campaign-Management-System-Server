namespace DAL.Models;

public class VoterLedgerFilterRecord
{
    public int IdNum { get; set; } 
    public string? LastName { get; set; } 
    public string? FirstName { get; set; } 
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
    public string? Appartment { get; set; } 
    public string? HouseLetter { get; set; } 
    public int? BallotSerial { get; set; } 
    public string? Spare3 { get; set; } 
    public string? Spare4 { get; set; } 
    public int? ZipCode { get; set; } 
    public string? Spare5 { get; set; } 
    public int CampaignYear { get; set; } 
    public string? Email1 { get; set; } 
    public string? Email2 { get; set; } 
    public string? Phone1 { get; set; } 
    public string? Phone2 { get; set; } 
    public double? InnerCityBallotId { get; set; } 
    public string? BallotAddress { get; set; } 
    public string? BallotLocation { get; set; } 
    public bool? Accessible { get; set; } 
    public int? ElligibleVoters { get; set; } 
    public bool? SupportStatus { get; set; } 
}