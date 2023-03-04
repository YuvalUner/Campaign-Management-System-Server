using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IPublishingService
{
    /// <summary>
    /// Publishes an event by adding it to the published events table
    /// </summary>
    /// <param name="eventGuid">Guid of the event to publish</param>
    /// <param name="publisherId">User id of the publishing user</param>
    /// <returns>Status code EventNotFound if the event does not exist, DuplicateKey if the event is already published,
    /// IncorrectEventType if event is not associated to any campaign, UserNotFound if publisher id is not a valid user</returns>
    Task<CustomStatusCode> PublishEvent(Guid? eventGuid, int? publisherId);
    
    /// <summary>
    /// Unpublishes an event by removing it from the published events table
    /// </summary>
    /// <param name="eventGuid">The Guid of the event to unpublish</param>
    /// <returns>Status code EventNotFound if event is not currently published</returns>
    Task<CustomStatusCode> UnpublishEvent(Guid? eventGuid);

    /// <summary>
    /// Gets all published events for a campaign, along with the user who published them.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign to get the published events for.</param>
    /// <returns>Status code CampaignNotFound if the campaign does not exist.</returns>
    Task<(CustomStatusCode, IEnumerable<PublishedEventWithPublisher>)> GetCampaignPublishedEvents(
        Guid? campaignGuid);

    /// <summary>
    /// Publishes an announcement by adding it to the published announcements table.
    /// </summary>
    /// <param name="announcement">An Announcement object with at-least the content, title and publisher id fields filled.</param>
    /// <param name="campaignGuid">Guid of the campaign the announcement is related to.</param>
    /// <returns>In item 1: Status code CampaignNotFound if the campaign does not exist, UserNotFound if publisher id is not a valid user.<br/>
    /// In item 2: Guid of new announcements. Empty Guid if stored procedure failed.</returns>
    Task<(CustomStatusCode, Guid)> PublishAnnouncement(Announcement announcement, Guid campaignGuid);

    /// <summary>
    /// Unpublishes an announcement by removing it from the published announcements table.
    /// </summary>
    /// <param name="announcementGuid">The Guid of the announcement to remove.</param>
    /// <returns>Status code AnnouncementNotFound if the announcement does not exist.</returns>
    Task<CustomStatusCode> UnpublishAnnouncement(Guid? announcementGuid);

    /// <summary>
    /// Gets all published announcements for a campaign, along with the user who published them.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign to get announcements for.</param>
    /// <returns>Status code CampaignNotFound if the campaign does not exist.</returns>
    Task<(CustomStatusCode, IEnumerable<AnnouncementWithPublisherDetails>)>
        GetCampaignAnnouncements(Guid? campaignGuid);
}