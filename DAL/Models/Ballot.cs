namespace DAL.Models;

/// <summary>
/// A model representing a single ballot, similar to the one stored in the DB's ballots table.<br/>
/// This one only contains the information that can be shown to the public.<br/>
/// </summary>
public class Ballot
{
    /// <summary>
    /// A decimal value of the id of the ballot within the city it is in.<br/>
    /// For example, 57.1, 6.3, etc - these ids are unique within the city, but not globally.<br/>
    /// </summary>
    public Decimal? InnerCityBallotId { get; set; }

    public string? CityName { get; set; }

    public string? BallotAddress { get; set; }

    /// <summary>
    /// The physical landmark the ballot is in.<br/>
    /// For example, the name of a school, the name of a building, etc.<br/>
    /// </summary>
    public string? BallotLocation { get; set; }

    /// <summary>
    /// Whether the ballot is accessible to people with disabilities.<br/>
    /// True if it is accessible, false if it is not, null if it is unknown.<br/>
    /// </summary>
    public bool? Accessible { get; set; }

    public bool? IsCustomBallot { get; set; } = false;
    
    public int? ElligibleVoters { get; set; }
}