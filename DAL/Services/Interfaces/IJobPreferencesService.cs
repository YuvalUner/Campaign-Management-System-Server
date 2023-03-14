using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for CRUD operations on user job preferences.
/// </summary>
public interface IJobPreferencesService
{
    /// <summary>
    /// Adds a new job preference for a user.
    /// </summary>
    /// <param name="userId">User id of the user to add the preference for.</param>
    /// <param name="campaignGuid">Guid of the campaign the preference relates to.</param>
    /// <param name="userPreferencesText">Text of the preference.</param>
    /// <returns></returns>
    Task AddUserPreferences(int? userId, Guid campaignGuid, string? userPreferencesText);

    /// <summary>
    /// Removes a user's job preference.
    /// </summary>
    /// <param name="userId">User id of the user to remove a job preference for.</param>
    /// <param name="campaignGuid">Guid of the campaign the preference relates to.</param>
    /// <returns></returns>
    Task DeleteUserPreferences(int? userId, Guid campaignGuid);

    /// <summary>
    /// Updates a user's existing job preference.
    /// </summary>
    /// <param name="userId">User id of the user to update the preference for.</param>
    /// <param name="campaignGuid">Guid of the campaign that the preference relates to.</param>
    /// <param name="userPreferencesText">New text to update to.</param>
    /// <returns></returns>
    Task UpdateUserPreferences(int? userId, Guid campaignGuid, string? userPreferencesText);

    /// <summary>
    /// Gets a user's job preference for a campaign.
    /// </summary>
    /// <param name="userId">The user id of the user to get the job preference for.</param>
    /// <param name="campaignGuid">The guid of the campaign the preference relates to.</param>
    /// <returns>A single instance of <see cref="UserJobPreference"/>, if one exists. Null otherwise.</returns>
    Task<UserJobPreference?> GetUserPreferences(int? userId, Guid campaignGuid);
}