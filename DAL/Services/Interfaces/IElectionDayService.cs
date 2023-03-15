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
}