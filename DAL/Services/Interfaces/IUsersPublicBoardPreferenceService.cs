using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for adding, removing, and getting preferences for a user regarding a campaign.
/// </summary>
public interface IUsersPublicBoardPreferenceService
{
    /// <summary>
    /// Adds a preference for a user regarding a campaign.
    /// </summary>
    /// <param name="userId">Id of the user to add a preference for.</param>
    /// <param name="campaignGuid">Guid of the campaign for which to add a preference.</param>
    /// <param name="isPreferred">set to true if the user wants to prioritize updates,
    /// and false if the user wants to avoid updates.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist,</returns>
    Task<CustomStatusCode> AddPreference(int? userId, Guid campaignGuid, bool isPreferred);

    /// <summary>
    /// Removes a preference for a user regarding a campaign.
    /// </summary>
    /// <param name="userId">The id of the user to add a preference for.</param>
    /// <param name="campaignGuid">The Guid of the campaign for which to remove a preference.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist,
    /// <see cref="CustomStatusCode.PreferenceNotFound"/> if the user does not have a preference for this campaign</returns>
    Task<CustomStatusCode> RemovePreference(int? userId, Guid campaignGuid);

    /// <summary>
    /// Updates a preference for a user regarding a campaign.
    /// </summary>
    /// <param name="userId">Id of the user to add a preference for.</param>
    /// <param name="campaignGuid">Guid of the campaign for which to add a preference.</param>
    /// <param name="isPreferred">set to true if the user wants to prioritize updates,
    /// and false if the user wants to avoid updates.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist,
    /// <see cref="CustomStatusCode.PreferenceNotFound"/> if the user does not have a preference for this campaign.</returns>
    Task<CustomStatusCode> UpdatePreference(int? userId, Guid campaignGuid, bool isPreferred);

    /// <summary>
    /// Gets all preferences for a user, including the name, logo and guid of each campaign.
    /// </summary>
    /// <param name="userId">The id of the user to get preferences for.</param>
    /// <returns>Item 1: Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist.<br/>
    /// Item 2: the list of user preferences. For details, check the return type <see cref="UserPreferenceResult"/>
    /// </returns>
    Task<(CustomStatusCode, IEnumerable<UserPreferenceResult>)> GetPreferences(int? userId);
}