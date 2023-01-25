using DAL.Models;

namespace DAL.Services.Interfaces;

public interface INotificationsService
{
    /// <summary>
    /// Adds a user to notify when someone joins a specific campaign.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="viaSms">True if they should be notified via sms, false otherwise</param>
    /// <param name="viaEmail">True if they want to be notified via email, false otherwise</param>
    /// <returns></returns>
    Task AddUserToNotify(int? userId, Guid campaignGuid, bool viaSms, bool viaEmail);

    /// <summary>
    /// Removes a user from being notified when someone joins a specific campaign.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task RemoveUserToNotify(int? userId, Guid campaignGuid);

    /// <summary>
    /// Modifies a user's preferences for being notified when someone joins a specific campaign.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="viaSms">True if they should be notified via sms, false otherwise</param>
    /// <param name="viaEmail">True if they want to be notified via email, false otherwise</param>
    /// <returns></returns>
    Task ModifyUserToNotify(int? userId, Guid campaignGuid, bool viaSms, bool viaEmail);

    /// <summary>
    /// Gets the users that should be notified when someone joins a specific campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<UserToNotify>> GetUsersToNotify(Guid campaignGuid);
}