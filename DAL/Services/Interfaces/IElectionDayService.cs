using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A collection of methods used for storing, retrieving, and modifying data related to the election day.
/// </summary>
public interface IElectionDayService
{
    Task<Ballot?> GetUserAssignedBallot(int? userId);
}