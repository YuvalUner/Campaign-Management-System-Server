﻿using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for adding, removing, and getting schedule managers for a user, as well as getting the list of users managed by a user.
/// </summary>
public interface IScheduleManagersService
{
    /// <summary>
    /// Gets the list of users who can manage the schedule of the given user.
    /// </summary>
    /// <param name="userEmail">Email of the user to get schedule managers for. This or userId must be provided.</param>
    /// <param name="userId">User Id of the user to get schedule managers for. This or userEmail must be provided.</param>
    /// <returns>Stored procedure's status code as item 1, list of schedule managers as item 2.<br/>
    /// Stored procedure's status code can be <see cref="CustomStatusCode.ParameterMustNotBeNullOrEmpty"/>
    /// if both userId and userEmail are empty,
    /// <see cref="CustomStatusCode.TooManyValuesProvided"/> if both userId and userEmail are provided</returns>
    Task<(CustomStatusCode, IEnumerable<User>)> GetScheduleManagers(string? userEmail = null, int? userId = null);

    /// <summary>
    /// Adds a schedule manager to the given user.
    /// </summary>
    /// <param name="giverUserId">The user id of the user that gives the schedule management permission</param>
    /// <param name="receiverEmail">The email of the user that receives the schedule management permission</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the permission receiver does not exist,
    /// <see cref="CustomStatusCode.DuplicateKey"/> if user is already a schedule manage of the requesting user</returns>
    Task<CustomStatusCode> AddScheduleManager(int giverUserId, string receiverEmail);

    /// <summary>
    /// Removes a schedule manager from the given user.
    /// </summary>
    /// <param name="giverUserId">The user id of the user that gave the schedule management permission.</param>
    /// <param name="receiverEmail">The user id of the user that has the schedule management permission.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the permission receiver does not exist</returns>
    Task<CustomStatusCode> RemoveScheduleManager(int giverUserId, string receiverEmail);

    /// <summary>
    /// Gets the list of users who are managed by the given user.
    /// </summary>
    /// <param name="userId">User id of the requesting user.</param>
    /// <returns>List of <see cref="UserPublicInfo"/> containing the public info of all managed users.</returns>
    Task<IEnumerable<UserPublicInfo>> GetManagedUsers(int userId);
}