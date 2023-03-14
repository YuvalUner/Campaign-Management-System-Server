using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for managing which users are notified when someone joins a campaign.
/// </summary>
public interface INotificationsService
{
    /// <summary>
    /// Adds a user to notify when someone joins a specific campaign.
    /// </summary>
    /// <param name="userId">User id of the user to add notification settings for.</param>
    /// <param name="campaignGuid">Guid of the campaign to notify for.</param>
    /// <param name="viaSms">True if they should be notified via sms, false otherwise</param>
    /// <param name="viaEmail">True if they want to be notified via email, false otherwise</param>
    /// <returns></returns>
    Task AddUserToNotify(int? userId, Guid campaignGuid, bool viaSms, bool viaEmail);

    /// <summary>
    /// Removes a user from being notified when someone joins a specific campaign.
    /// </summary>
    /// <param name="userId">User id of the user to remove notification settings for.</param>
    /// <param name="campaignGuid">Guid of the campaign it relates to.</param>
    /// <returns></returns>
    Task RemoveUserToNotify(int? userId, Guid campaignGuid);

    /// <summary>
    /// Modifies a user's preferences for being notified when someone joins a specific campaign.
    /// </summary>
    /// <param name="userId">User id of the user to add notification settings for.</param>
    /// <param name="campaignGuid">Guid of the campaign to notify for.</param>
    /// <param name="viaSms">True if they should be notified via sms, false otherwise</param>
    /// <param name="viaEmail">True if they want to be notified via email, false otherwise</param>
    /// <returns></returns>
    Task UpdateUserToNotify(int? userId, Guid campaignGuid, bool viaSms, bool viaEmail);

    /// <summary>
    /// Gets the users that should be notified when someone joins a specific campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>A list of <see cref="NotificationSettings"/>, to be used for sending the notifications.</returns>
    Task<IEnumerable<NotificationSettings>> GetUsersToNotify(Guid campaignGuid);
}