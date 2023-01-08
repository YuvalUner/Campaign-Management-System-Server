using DAL.Models;

namespace DAL.Services;

public interface IUsersService
{
    /// <summary>
    /// Gets the user with all of their information by their email address.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<User?> GetUserByEmail(string email);

    /// <summary>
    /// Creates a user in the database.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>The new identity id of the user on success, -1 on failure.</returns>
    Task<int> CreateUser(User user);
}