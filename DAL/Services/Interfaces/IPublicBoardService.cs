using DAL.Models;

namespace DAL.Services.Interfaces;

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
    /// <returns></returns>
    Task<IEnumerable<PublishedEventWithPublisher>> GetEventsForUser(int? userId, int? limit);
    
    /// <summary>
    /// Gets published announcements for a specific user, based on their preferences.<br/>
    /// Announcements are ordered such that announcements from preferred campaigns are returned first, followed by
    /// other campaigns, and all are ordered by publishing date.<br/>
    /// Announcements from avoided campaigns are filtered out.<br/>
    /// </summary>
    /// <param name="userId">Id of the user to match the preferences to. Keep null to not take preferences into account.</param>
    /// <param name="limit">How many rows to get. Keep null to get 50.</param>
    /// <returns></returns>
    Task<IEnumerable<AnnouncementWithPublisherDetails>> GetAnnouncementsForUser(int? userId, int? limit);

    /// <summary>
    /// Searches published events according to the given parameters.
    /// </summary>
    /// <param name="searchParams">The search parameters to use. For specification, see <see cref="EventsSearchParams"/>. 
    /// All fields left null will be ignored.</param>
    /// <returns></returns>
    Task<IEnumerable<PublishedEventWithPublisher>> SearchEvents(EventsSearchParams searchParams);

    /// <summary>
    /// Searches published announcements according to the given parameters.
    /// </summary>
    /// <param name="searchParams">The search parameters to use. For specification, see <see cref="AnnouncementSearchParams"/>.
    /// All fields left null will be ignored.</param>
    /// <returns></returns>
    Task<IEnumerable<AnnouncementWithPublisherDetails>> SearchAnnouncements(AnnouncementSearchParams searchParams);
}