using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A collection of methods used for storing, retrieving, and modifying data related to events.
/// </summary>
public interface IEventsService
{
    /// <summary>
    /// Adds a new custom event to the database, for either a campaign or a user.
    /// </summary>
    /// <param name="customEvent">An instance of <see cref="CustomEvent"/> with the needed fields filled in.</param>
    /// <returns>Status code <see cref="CustomStatusCode.CampaignNotFound"/> if campaign was provided but not found.
    /// If successful, returns the new event's id and Guid in items 2 and 3 of the tuple.</returns>
    Task<(CustomStatusCode, int?, Guid?)> AddEvent(CustomEvent customEvent);
    
    /// <summary>
    /// Updates an existing event in the database.
    /// </summary>
    /// <param name="customEvent">An instance of <see cref="CustomEvent"/> with the fields that should be updated filled in,
    /// and eventGuid always filled in.</param>
    /// <returns>Status code <see cref="CustomStatusCode.EventNotFound"/> if the event does not exist,
    /// Status code <see cref="CustomStatusCode.CampaignNotFound"/> if campaign was provided but not found.</returns>
    Task<CustomStatusCode> UpdateEvent(CustomEvent customEvent);
    
    /// <summary>
    /// Deletes an event from the database.
    /// </summary>
    /// <param name="eventGuid">Guid of the event to delete.</param>
    /// <returns>Status code <see cref="CustomStatusCode.EventNotFound"/> if the event does not exist.</returns>
    Task<CustomStatusCode> DeleteEvent(Guid eventGuid);

    /// <summary>
    /// Adds a user to an event as a participant. Removes the user from the list of watchers if they are already
    /// a watcher.
    /// </summary>
    /// <param name="eventGuid">Guid of the event to add the participant to.</param>
    /// <param name="userId">User id of the user to add - either this or userEmail must be provided.</param>
    /// <param name="userEmail">Email of the user to add - either this or userId must be provided.</param>
    /// <returns><see cref="CustomStatusCode.EventNotFound"/> if the event does not exist,
    /// <see cref="CustomStatusCode.UserNotFound"/> if the user does not exist,
    /// <see cref="CustomStatusCode.ParameterMustNotBeNullOrEmpty"/> if both userId and userEmail are empty,
    /// <see cref="CustomStatusCode.TooManyValuesProvided"/> if both
    /// userId and userEmail are provided, <see cref="CustomStatusCode.DuplicateKey"/> if user is already assigned to the event.</returns>
    Task<CustomStatusCode> AddEventParticipant(Guid eventGuid, int? userId = null, string? userEmail = null);

    /// <summary>
    /// Gets all events for a user, where they are either a participant or a watcher.
    /// </summary>
    /// <param name="userId">User id of the user to get the events for.</param>
    /// <returns>A list of <see cref="GetUserEventsResult"/>, each entry containing info about the event.</returns>
    Task<IEnumerable<GetUserEventsResult>> GetUserEvents(int userId);

    /// <summary>
    /// Adds the user as a watcher of the event. Removes the user from the list of participants if they are already
    /// a participant.
    /// </summary>
    /// <param name="userId">user id of the user to add as a watcher.</param>
    /// <param name="eventGuid">Guid of the event.</param>
    /// <returns><see cref="CustomStatusCode.EventNotFound"/> if the event does not exist,
    /// <see cref="CustomStatusCode.DuplicateKey"/> if user is already a watcher of that event.</returns>
    Task<CustomStatusCode> AddEventWatcher(int userId, Guid eventGuid);

    /// <summary>
    /// Removes a user from the list of watchers of an event.
    /// </summary>
    /// <param name="userId">User id of the user to remove as a watcher.</param>
    /// <param name="eventGuid">Guid of the event.</param>
    /// <returns>EventNotFound if the event does not exist.</returns>
    Task<CustomStatusCode> RemoveEventWatcher(int userId, Guid eventGuid);

    /// <summary>
    /// Removes a user from the list of participants of an event.
    /// </summary>
    /// <param name="eventGuid">Guid of the event.</param>
    /// <param name="userId">User id of the user to remove as a participant - either this or userEmail must be provided.</param>
    /// <param name="userEmail">Email of the user to remove as a participant - either this or userId must be provided.</param>
    /// <returns><see cref="CustomStatusCode.EventNotFound"/> if the event does not exist, <see cref="CustomStatusCode.UserNotFound"/>
    /// if the user does not exist, <see cref="CustomStatusCode.ParameterMustNotBeNullOrEmpty"/> if both userId and userEmail are empty,
    /// <see cref="CustomStatusCode.TooManyValuesProvided"/> if both userId and userEmail are provided.</returns>
    Task<CustomStatusCode> RemoveEventParticipant(Guid eventGuid, int? userId = null, string? userEmail = null);

    /// <summary>
    /// Gets the list of events for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get the list of events for.</param>
    /// <returns>A list of <see cref="EventWithCreatorDetails"/>, each entry containing details about the event and its creator.</returns>
    Task<IEnumerable<EventWithCreatorDetails?>> GetCampaignEvents(Guid? campaignGuid);

    /// <summary>
    /// Gets the list of participants for a specific event.
    /// </summary>
    /// <param name="eventGuid">Guid of the event.</param>
    /// <returns>Item 1: <see cref="CustomStatusCode.EventNotFound"/> if the event does not exist.<br/>
    /// Item 2: A list of <see cref="UserPublicInfo"/>, each entry containing info about each user.</returns>
    Task<(CustomStatusCode, IEnumerable<UserPublicInfo>)> GetEventParticipants(Guid eventGuid);

    /// <summary>
    /// Gets the details of a single event, along with its creator's details.
    /// </summary>
    /// <param name="eventGuid">Guid of the event.</param>
    /// <returns>An instance of <see cref="EventWithCreatorDetails"/> if the event was found, null otherwise.</returns>
    Task<EventWithCreatorDetails?> GetEvent(Guid eventGuid);

    /// <summary>
    /// Returns the id of the user who created the event.
    /// </summary>
    /// <param name="eventGuid">Guid of the event.</param>
    /// <returns>An instance of <see cref="User"/> with only the userId property set.</returns>
    Task<User?> GetEventCreatorUserId(Guid eventGuid);

    /// <summary>
    /// Gets a list of all the user's personal events.
    /// </summary>
    /// <param name="userId">User id of the user.</param>
    /// <returns>An enumerable of <see cref="EventWithCreatorDetails"/>, with each entry containing info about the events.</returns>
    Task<IEnumerable<EventWithCreatorDetails>> GetPersonalEvents(int userId);

    /// <summary>
    /// Gets the list of watchers for a specific event.
    /// </summary>
    /// <param name="eventGuid">Guid of the event.</param>
    /// <returns>Item 1: Always <see cref="CustomStatusCode.Ok"/>, just an implementation detail that ended up being
    /// redundant but required too much effort to fix.<br/>
    /// Item 2: A list of <see cref="UserPublicInfo"/>, each entry containing info about each user.
    /// </returns>
    Task<(CustomStatusCode, IEnumerable<UserPublicInfo>)> GetEventWatchers(Guid eventGuid);

}