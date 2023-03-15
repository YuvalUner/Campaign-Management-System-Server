namespace DAL.Models;

/// <summary>
/// A model for a single row of the voters ledger table.<br/>
/// The fields are named according to the names of the columns in the table.<br/>
/// The spare fields are because they can be provided by the official voters ledger, but are generally not used, so it
/// is easier to ignore them (this line has been written because I tried figuring out what the spare fields are for way too long,
/// so I would like to save the next person the trouble).
/// </summary>
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