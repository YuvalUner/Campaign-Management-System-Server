﻿using DAL.Models;

namespace DAL.Services.Interfaces;

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

    /// <summary>
    /// Gets all the campaigns a user is in.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>List of CampaignUser objects, each with a campaign and the role the user has in that campaign.</returns>
    Task<List<CampaignUser>> GetUserCampaigns(int? userId);

    /// <summary>
    /// Gets the public info of a user - their name and profile picture.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<User?> GetUserPublicInfo(int? userId);

    /// <summary>
    /// Adds the user's private info to the database in the relevant tables.
    /// </summary>
    /// <param name="privateInfo">The model with the private info</param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<int> AddUserPrivateInfo(UserPrivateInfo privateInfo, int? userId);

    /// <summary>
    /// Gets a user's authentication status from the database and returns it.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> IsUserAuthenticated(int? userId);

    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteUser(int? userId);

    /// <summary>
    /// Adds a phone number to the user's account.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task AddPhoneNumber(int? userId, string phoneNumber);
    
    /// <summary>
    /// Gets the user's phone and email, if they exist.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<User?> GetUserContactInfo(int? userId);

    Task<User?> GetUserContactInfoByEmail(string userEmail);
}