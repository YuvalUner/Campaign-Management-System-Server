namespace DAL.Models;

public class Ballot
{
    public Decimal? InnerCityBallotId { get; set; }
    public string? CityName { get; set; }
    public string? BallotAddress { get; set; }
    public string? BallotLocation  { get; set; }
    public bool? Accessible { get; set; }
}