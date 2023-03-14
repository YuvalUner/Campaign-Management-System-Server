using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for getting published events and announcements, as well as searching for them and also
/// for adding, removing, and updating notification settings for publishings for a user.
/// </summary>
public interface IPublicBoardService
{
    /// <summary>
    /// Gets published events for a specific user, based on their preferences.<br/>
    /// Events are ordered such that events from preferred campaigns are returned first, followed by events from
    /// other campaigns, and all are ordered by publishing date.<br/>
    /// Events from avoided campaigns are filtered out.<br/>
    /// </summary>
    /// <param name="userId">Id of the user to match the preferences to. Keep null to not take preferences into account.</param>
    /// <param name="limit">How many rows to get. Keep null to get 50.</param>
    /// <param name="offset">Row offset. Used to "scroll" the feed. Keep null for offset 0.</param>
    /// <returns>A list of <see cref="PublishedEventWithPublisher"/>, with the event and info on the publisher for each
    /// event.</returns>
    Task<IEnumerable<PublishedEventWithPublisher>> GetEventsForUser(int? userId, int? limit, int? offset);

    /// <summary>
    /// Gets published announcements for a specific user, based on their preferences.<br/>
    /// Announcements are ordered such that announcements from preferred campaigns are returned first, followed by
    /// other campaigns, and all are ordered by publishing date.<br/>
    /// Announcements from avoided campaigns are filtered out.<br/>
    /// </summary>
    /// <param name="userId">Id of the user to match the preferences to. Keep null to not take preferences into account.</param>
    /// <param name="limit">How many rows to get. Keep null to get 50.</param>
    /// <param name="offset">Row offset. Used to "scroll" the feed. Keep null for offset 0.</param>
    /// <returns>A list of <see cref="AnnouncementWithPublisherDetails"/> with details on the announcement and its publisher
    /// in each entry.</returns>
    Task<IEnumerable<AnnouncementWithPublisherDetails>> GetAnnouncementsForUser(int? userId, int? limit, int? offset);

    /// <summary>
    /// Searches published events according to the given parameters.
    /// </summary>
    /// <param name="searchParams">The search parameters to use. For specification, see <see cref="EventsSearchParams"/>. 
    /// All fields left null will be ignored.</param>
    /// <returns>A list of <see cref="PublishedEventWithPublisher"/>s that match the filter.</returns>
    Task<IEnumerable<PublishedEventWithPublisher>> SearchEvents(EventsSearchParams searchParams);

    /// <summary>
    /// Searches published announcements according to the given parameters.
    /// </summary>
    /// <param name="searchParams">The search parameters to use. For specification, see <see cref="AnnouncementsSearchParams"/>.
    /// All fields left null will be ignored.</param>
    /// <returns>A list of <see cref="AnnouncementWithPublisherDetails"/>s that match the filters.</returns>
    Task<IEnumerable<AnnouncementWithPublisherDetails>> SearchAnnouncements(AnnouncementsSearchParams searchParams);

    /// <summary>
    /// Adds a new notification settings entry for a user.
    /// </summary>
    /// <param name="settings">An instance of <see cref="NotificationUponPublishSettings"/> containing the settings required,
    /// as well as the user id and campaign Guid.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/>  if the user does not exist,
    /// <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist.</returns>
    Task<CustomStatusCode> AddNotificationSettings(NotificationUponPublishSettings settings);

    /// <summary>
    /// Removes a notification settings entry for a user.
    /// </summary>
    /// <param name="settings">An instance of <see cref="NotificationUponPublishSettings"/> with userId and campaignGuid filled in.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist.</returns>
    Task<CustomStatusCode> RemoveNotificationSettings(NotificationUponPublishSettings settings);

    /// <summary>
    /// Updates an existing notification settings entry for a user.
    /// </summary>
    /// <param name="settings">An instance of <see cref="NotificationUponPublishSettings"/> containing the settings required.</param>
    /// <returns>Status code <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.CampaignNotFound"/> if the campaign does not exist.</returns>
    Task<CustomStatusCode> UpdateNotificationSettings(NotificationUponPublishSettings settings);

    /// <summary>
    /// Gets the notification settings for a user.
    /// </summary>
    /// <param name="userId">User id of the user to get the settings for.</param>
    /// <returns>A list of <see cref="NotificationUponPublishSettingsForUser"/> with the information about each campaign
    /// the user is subscribed to.</returns>
    Task<IEnumerable<NotificationUponPublishSettingsForUser>> GetNotificationSettingsForUser(int userId);

    /// <summary>
    /// Gets the notification settings of every user subscribed to a campaign.
    /// </summary>
    /// <param name="campaignGuid">The campaign to get the settings for.</param>
    /// <returns>A list of <see cref="NotificationUponPublishSettingsForCampaign"/> that includes the name and contact info
    /// of each user.</returns>
    Task<IEnumerable<NotificationUponPublishSettingsForCampaign>> GetNotificationSettingsForCampaign(Guid campaignGuid);
}