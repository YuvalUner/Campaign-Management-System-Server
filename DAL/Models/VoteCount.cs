namespace DAL.Models;

public class VoteCount
{
    public int? BallotId { get; set; }
    public bool? IsCustomBallot { get; set; }
    public int? PartyId { get; set; }
    public int? NumVotes { get; set; }
}