using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A collection of methods used for storing, retrieving, and modifying data related to the election day.
/// </summary>
public interface IElectionDayService
{
    /// <summary>
    /// Gets details on the ballot assigned to a specific user.
    /// </summary>
    /// <param name="userId">User id of the user.</param>
    /// <returns>A <see cref="Ballot"/> object with all the fields filled in, so long as one can be associated with the
    /// given user id.</returns>
    Task<Ballot?> GetUserAssignedBallot(int? userId);

    /// <summary>
    /// Gets the vote count of all parties according to the given campaign guid.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<VoteCount>> GetVoteCount(Guid campaignGuid);

    /// <summary>
    /// Modifies the vote count of a specific party in a specific ballot.
    /// </summary>
    /// <param name="voteCount"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="increment">Whether to increment or decrement the vote count. True to increment, false to decrement.</param>
    /// <returns>CampaignNotFound for bad campaign guid, BallotNotFound if the ballot does not exist,
    /// PartyNotFound if party with the given id does not exist, Ok otherwise.</returns>
    Task<CustomStatusCode> ModifyVoteCount(VoteCount voteCount, Guid campaignGuid, bool increment);
}