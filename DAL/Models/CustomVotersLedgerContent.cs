namespace DAL.Models;

public class CustomVotersLedgerContent
{
    public int? Identifier { get; set; } 
    public int? LedgerId { get; set; } 
    public string? LastName { get; set; } 
    public string? FirstName { get; set; }
    public string? CityName { get; set; } 
    public int? BallotId { get; set; }
    public string? StreetName { get; set; } 
    public int? HouseNumber { get; set; } 
    public string? Entrance { get; set; } 
    public string? Appartment { get; set; } 
    public string? HouseLetter { get; set; } 
    public int? ZipCode { get; set; }
    public string? Email1 { get; set; }
    public string? Email2 { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public bool? SupportStatus { get; set; }
}
