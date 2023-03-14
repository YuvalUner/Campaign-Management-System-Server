using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for adding, removing, and getting various information about users.
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Gets the user with all of their information by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>An instance of <see cref="User"/> if a user with that email address exists.</returns>
    Task<User?> GetUserByEmail(string email);

    /// <summary>
    /// Creates a user in the database.
    /// </summary>
    /// <param name="user">An instance of <see cref="User"/> with all of their required information filled in.</param>
    /// <returns>The new identity id of the user on success, -1 on failure.</returns>
    Task<int> CreateUser(User user);

    /// <summary>
    /// Gets all the campaigns a user is in.
    /// </summary>
    /// <param name="userId">The user id of the user to get the list of campaigns for.</param>
    /// <returns>List of <see cref="CampaignUser"/> objects, each with a campaign and the role the user has in that campaign.</returns>
    Task<List<CampaignUser>> GetUserCampaigns(int? userId);

    /// <summary>
    /// Gets the public info of a user - their name and profile picture.
    /// </summary>
    /// <param name="userId">User id of the user to get the public info for.</param>
    /// <returns>An instance of <see cref="User"/> with only name and profile picture fields set to not null.</returns>
    Task<User?> GetUserPublicInfo(int? userId);

    /// <summary>
    /// Adds the user's private info to the database in the relevant tables.
    /// </summary>
    /// <param name="privateInfo">An instance of <see cref="UserPrivateInfo"/> with the private info filled in.</param>
    /// <param name="userId">User id of the user to add the info for.</param>
    /// <returns><see cref="CustomStatusCode.IdAlreadyExistsWhenVerifyingInfo"/> if a user with the same id as an
    /// already verified user tries to verify, which may indicate a malicious user or an id being stolen.</returns>
    Task<CustomStatusCode> AddUserPrivateInfo(UserPrivateInfo privateInfo, int? userId);

    /// <summary>
    /// Gets a user's authentication status from the database and returns it.
    /// </summary>
    /// <param name="userId">User id of the user for which to query their authentication status.</param>
    /// <returns>True if the user is authenticated, false otherwise.</returns>
    Task<bool> IsUserAuthenticated(int? userId);

    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <param name="userId">User id of the user to delete.</param>
    /// <returns></returns>
    Task DeleteUser(int? userId);

    /// <summary>
    /// Adds a phone number to the user's account.
    /// </summary>
    /// <param name="userId">User id of the user to add a phone number to.</param>
    /// <param name="phoneNumber">The phone number to add.</param>
    /// <returns></returns>
    Task AddPhoneNumber(int? userId, string phoneNumber);
    
    /// <summary>
    /// Gets the user's phone and email, if they exist.
    /// </summary>
    /// <param name="userId">User id to query.</param>
    /// <returns>An instance of <see cref="User"/> with only the phone and email set to not null.</returns>
    Task<User?> GetUserContactInfo(int? userId);

    /// <summary>
    /// Used to get the user's phone number when their email is available.
    /// </summary>
    /// <param name="userEmail">The user's email address.</param>
    /// <returns>An instance of <see cref="User"/> with only the phone and email set to not null.</returns>
    Task<User?> GetUserContactInfoByEmail(string userEmail);
    
    /// <summary>
    /// Removes a user's phone number from their account.
    /// </summary>
    /// <param name="userId">The user id of the user to remove the phone number from.</param>
    /// <returns></returns>
    Task RemovePhoneNumber(int? userId);
}