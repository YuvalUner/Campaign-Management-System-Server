using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IEventsService
{
    /// <summary>
    /// Adds a new custom event to the database, for either a campaign or a user.
    /// </summary>
    /// <param name="customEvent"></param>
    /// <returns>Status code CampaignNotFound if campaign was provided but not found.
    /// If successful, returns the new event's id and Guid in items 2 and 3 of the tuple.</returns>
    Task<(CustomStatusCode, int?, Guid?)> AddEvent(CustomEvent customEvent);
    
    /// <summary>
    /// Updates an existing event in the database.
    /// </summary>
    /// <param name="customEvent"></param>
    /// <returns>Status code EventNotFound if the event does not exist, Status code CampaignNotFound if campaign was
    /// provided but not found.</returns>
    Task<CustomStatusCode> UpdateEvent(CustomEvent customEvent);
    
    /// <summary>
    /// Deletes an event from the database.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <returns>Status code EventNotFound if the event does not exist.</returns>
    Task<CustomStatusCode> DeleteEvent(Guid eventGuid);

    /// <summary>
    /// Adds a user to an event as a participant. Removes the user from the list of watchers if they are already
    /// a watcher.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <param name="userId"></param>
    /// <param name="userEmail"></param>
    /// <returns>EventNotFound if the event does not exist, UserNotFound if the user does not exist,
    /// ParameterMustNotBeNullOrEmpty if both userId and userEmail are empty, TooManyValuesProvided if both
    /// userId and userEmail are provided, DuplicateKey if user is already assigned to the event.</returns>
    Task<CustomStatusCode> AddEventParticipant(Guid eventGuid, int? userId = null, string? userEmail = null);

    /// <summary>
    /// Gets all events for a user, where they are either a participant or a watcher.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<GetUserEventsResult>> GetUserEvents(int userId);

    /// <summary>
    /// Adds the user as a watcher of the event. Removes the user from the list of participants if they are already
    /// a participant.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="eventGuid"></param>
    /// <returns>EventNotFound if the event does not exist, DuplicateKey if user is already a watcher of that event.</returns>
    Task<CustomStatusCode> AddEventWatcher(int userId, Guid eventGuid);

    /// <summary>
    /// Removes a user from the list of watchers of an event.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="eventGuid"></param>
    /// <returns>EventNotFound if the event does not exist.</returns>
    Task<CustomStatusCode> RemoveEventWatcher(int userId, Guid eventGuid);

    /// <summary>
    /// Removes a user from the list of participants of an event.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <param name="userId"></param>
    /// <param name="userEmail"></param>
    /// <returns>EventNotFound if the event does not exist, UserNotFound if the user does not exist,
    /// ParameterMustNotBeNullOrEmpty if both userId and userEmail are empty, TooManyValuesProvided if both
    /// userId and userEmail are provided.</returns>
    Task<CustomStatusCode> RemoveEventParticipant(Guid eventGuid, int? userId = null, string? userEmail = null);

    /// <summary>
    /// Gets the list of events for a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<EventWithCreatorDetails?>> GetCampaignEvents(Guid? campaignGuid);

    /// <summary>
    /// Gets the list of participants for a specific event.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <returns>EventNotFound if the event does not exist.</returns>
    Task<(CustomStatusCode, IEnumerable<User>)> GetEventParticipants(Guid eventGuid);

    /// <summary>
    /// Gets the details of a single event, along with its creator's details.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <returns></returns>
    Task<EventWithCreatorDetails?> GetEvent(Guid eventGuid);

    /// <summary>
    /// Returns the id of the user who created the event.
    /// </summary>
    /// <param name="eventGuid"></param>
    /// <returns></returns>
    Task<User?> GetEventCreatorUserId(Guid eventGuid);

}