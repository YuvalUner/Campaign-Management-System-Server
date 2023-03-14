using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;
using static API.Utils.ErrorMessages;

// Disable warnings for possibly null values - we know that the values are not null, but the compiler doesn't
// (due to models defining them as nullable)
#pragma warning disable CS4014
#pragma warning disable CS8629
#pragma warning disable CS8602
// Disable warnings for not awaiting async methods - they are not awaited on purpose.
#pragma warning disable CS8604

namespace API.Controllers;

/// <summary>
/// A controller for handling all requests related to events.<br/>
/// Generally, provides a web API and service policy for <see cref="IEventsService"/>,
/// along with other functionality related to events.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : Controller
{
    private readonly IEventsService _eventsService;
    private readonly ILogger<EventsController> _logger;
    private readonly IUsersService _usersService;
    private readonly IScheduleManagersService _scheduleManagersService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly ISmsMessageSendingService _smsMessageSendingService;
    private readonly ICampaignsService _campaignsService;

    public EventsController(IEventsService eventsService, ILogger<EventsController> logger, IUsersService usersService,
        IScheduleManagersService scheduleManagersService, IEmailSendingService emailSendingService,
        ISmsMessageSendingService smsMessageSendingService, ICampaignsService campaignsService)
    {
        _eventsService = eventsService;
        _logger = logger;
        _usersService = usersService;
        _scheduleManagersService = scheduleManagersService;
        _emailSendingService = emailSendingService;
        _smsMessageSendingService = smsMessageSendingService;
        _campaignsService = campaignsService;
    }

    /// <summary>
    /// A method for sending a message to users who were assigned to an event.<br/>
    /// Sends an email and/or an SMS message to the users, depending on the parameters.<br/>
    /// Generally, do not await this method as there is no need to wait for the messages to be sent.
    /// </summary>
    /// <param name="viaEmail">Whether to send an email or not.</param>
    /// <param name="viaSms">Whether to send an sms or not.</param>
    /// <param name="email">The email address of the user to send messages to.</param>
    /// <param name="customEvent">A <see cref="CustomEvent"/> containing the information to be sent to the user.</param>
    /// <param name="senderName">Name of the sender - a campaign's name or individual's name.</param>
    private async Task SendEventAssignedMessage(bool viaEmail, bool viaSms, string? email, CustomEvent customEvent,
        string? senderName = null)
    {
        if (viaEmail)
        {
            await _emailSendingService.SendAddedEventParticipationEmailAsync(
                customEvent.EventName,
                customEvent.EventLocation,
                customEvent.EventStartTime,
                customEvent.EventEndTime,
                email,
                senderName
            );
        }

        if (viaSms)
        {
            var user = await _usersService.GetUserContactInfoByEmail(email);
            if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
            {
                await _smsMessageSendingService.SendAddedEventParticipationSmsAsync(
                    customEvent.EventName,
                    customEvent.EventLocation,
                    customEvent.EventStartTime,
                    customEvent.EventEndTime,
                    user.PhoneNumber,
                    CountryCodes.Israel
                );
            }
        }
    }

    /// <summary>
    /// A method for informing users that an event was created.<br/>
    /// Sends an email and/or an SMS message to the users, depending on the parameters.<br/>
    /// Generally, do not await this method as there is no need to wait for the messages to be sent.
    /// </summary>
    /// <param name="viaEmail">Whether to send an email or not.</param>
    /// <param name="viaSms">Whether to send an sms or not.</param>
    /// <param name="email">The email address of the user to send messages to.</param>
    /// <param name="customEvent">A <see cref="CustomEvent"/> containing the information to be sent to the user.</param>
    /// <param name="creatorName">Name of the creator of the event - can be a campaign's name or individual's name.</param>
    /// <param name="senderName">Name of the sender - a campaign's name or individual's name.</param>
    private async Task SendEventCreatedForUserMessage(bool viaEmail, bool viaSms, string? email,
        CustomEvent customEvent,
        string? creatorName = null, string? senderName = null)
    {
        if (viaEmail)
        {
            await _emailSendingService.SendEventCreatedForUserEmailAsync(
                customEvent.EventName,
                customEvent.EventLocation,
                customEvent.EventStartTime,
                customEvent.EventEndTime,
                creatorName,
                email,
                senderName
            );
        }

        if (viaSms)
        {
            var user = await _usersService.GetUserContactInfoByEmail(email);

            if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
            {
                await _smsMessageSendingService.SendEventCreatedForUserSmsAsync(
                    customEvent.EventName,
                    customEvent.EventLocation,
                    customEvent.EventStartTime,
                    customEvent.EventEndTime,
                    creatorName,
                    user.PhoneNumber,
                    CountryCodes.Israel
                );
            }
        }
    }

    /// <summary>
    /// A method for informing users that an event was deleted.<br/>
    /// Sends an email and/or an SMS message to the users, depending on the parameters.<br/>
    /// Generally, do not await this method as there is no need to wait for the messages to be sent.
    /// </summary>
    /// <param name="viaEmail">Whether to send an email or not.</param>
    /// <param name="viaSms">Whether to send an sms or not.</param>
    /// <param name="email">The email address of the user to send messages to.</param>
    /// <param name="phoneNumber">The phone number of the user to send the message to.</param>
    /// <param name="customEvent">An <see cref="EventWithCreatorDetails"/> containing the information to be sent to the user.</param>
    /// <param name="senderName">Name of the sender - a campaign's name or individual's name.</param>
    private async Task SendEventDeletedMessage(bool viaEmail, bool viaSms, string? email, string? phoneNumber,
        EventWithCreatorDetails customEvent, string? senderName = null)
    {
        if (viaEmail)
        {
            await _emailSendingService.SendEventDeletedEmailAsync(
                customEvent.EventName,
                customEvent.EventLocation,
                customEvent.EventStartTime,
                customEvent.EventEndTime,
                email,
                senderName
            );
        }

        if (viaSms && !string.IsNullOrEmpty(phoneNumber))
        {
            await _smsMessageSendingService.SendEventDeletedMessageAsync(
                customEvent.EventName,
                customEvent.EventLocation,
                customEvent.EventStartTime,
                customEvent.EventEndTime,
                phoneNumber,
                senderName,
                CountryCodes.Israel
            );
        }
    }

    /// <summary>
    /// A method for informing users that an event was updated.<br/>
    /// Sends an email and/or an SMS message to the users, depending on the parameters.<br/>
    /// Generally, do not await this method as there is no need to wait for the messages to be sent.
    /// </summary>
    /// <param name="viaEmail">Whether to send an email or not.</param>
    /// <param name="viaSms">Whether to send an sms or not.</param>
    /// <param name="email">The email address of the user to send messages to.</param>
    /// <param name="phoneNumber">The phone number of the user to send the message to.</param>
    /// <param name="customEvent">A <see cref="CustomEvent"/> containing the information to be sent to the user.</param>
    /// <param name="senderName">Name of the sender - a campaign's name or individual's name.</param>
    private async Task SendEventUpdatedMessage(bool viaEmail, bool viaSms, string? email, string? phoneNumber,
        CustomEvent customEvent, string? senderName = null)
    {
        if (viaEmail)
        {
            await _emailSendingService.SendEventUpdatedEmailAsync(
                customEvent.EventName,
                customEvent.EventLocation,
                customEvent.EventStartTime,
                customEvent.EventEndTime,
                email,
                senderName
            );
        }

        if (viaSms && !string.IsNullOrEmpty(phoneNumber))
        {
            await _smsMessageSendingService.SendEventUpdatedMessageAsync(
                customEvent.EventName,
                customEvent.EventLocation,
                customEvent.EventStartTime,
                customEvent.EventEndTime,
                phoneNumber,
                senderName,
                CountryCodes.Israel
            );
        }
    }

    /// <summary>
    /// Gets the list of events the user is a part of (either as a participant or a watcher).
    /// </summary>
    /// <returns>Ok with the list of events as a JSON object, with each event being an instance of <see cref="GetUserEventsResult"/>.</returns>
    [HttpGet("get-self-events")]
    public async Task<IActionResult> GetSelf()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            var events = await _eventsService.GetUserEvents(userId.Value);
            return Ok(events);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting self events");
            return BadRequest("Error while getting self events");
        }
    }

    /// <summary>
    /// Gets the list of events that are part of the campaign, sorted by creation date.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign to get the events for.</param>
    /// <returns>Unauthorized if the user is not authorized to view the list of campaign events, Ok
    /// with a list of <see cref="EventWithCreatorDetails"/> as a JSON object otherwise.</returns>
    [HttpGet("get-campaign-events/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignEvents(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                    campaignGuid, new Permission()
                    {
                        PermissionTarget = PermissionTargets.Events,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var events = await _eventsService.GetCampaignEvents(campaignGuid);
            return Ok(events);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting campaign events");
            return StatusCode(500, "Error while getting campaign events");
        }
    }

    /// <summary>
    /// Creates a new personal event for the user.
    /// </summary>
    /// <param name="newEvent">A <see cref="CustomEvent"/> object, with all the required fields filled in.</param>
    /// <returns>BadRequest if some information is missing, Ok with the new event's Guid otherwise.</returns>
    [HttpPost("create-personal-event")]
    public async Task<IActionResult> CreatePersonalEvent([FromBody] CustomEvent newEvent)
    {
        try
        {
            // Check that the required fields are not null or empty.
            if (String.IsNullOrWhiteSpace(newEvent.EventName))
            {
                return BadRequest(FormatErrorMessage(EventNameIsRequired, CustomStatusCode.ValueNullOrEmpty));
            }

            if (newEvent.MaxAttendees is null or <= 0)
            {
                return BadRequest(FormatErrorMessage(MaxAttendeesNotNullOrZero, CustomStatusCode.ValueNullOrEmpty));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            newEvent.EventCreatorId = userId.Value;
            newEvent.CampaignGuid = null;
            newEvent.EventOf = userId.Value;

            // If the user did not specify whether the event is open join, then it is not.
            newEvent.IsOpenJoin ??= false;

            // The event is always created successfully, so long as the event name is not empty and campaignGuid is null.
            var (_, _, eventGuid) = await _eventsService.AddEvent(newEvent);

            // No need to await this - all of its fail conditions can not happen, since the event was just created
            // and its Guid is 100% correct.
            // User is always added as a participant to their own event.
            _eventsService.AddEventParticipant(eventGuid.Value, userId.Value);

            return Ok(new { eventGuid });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating personal event");
            return BadRequest("Error while creating personal event");
        }
    }

    /// <summary>
    /// Creates a new event for a campaign.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign to add the event for.</param>
    /// <param name="newEvent">A <see cref="CustomEvent"/> object with the required fields filled in.</param>
    /// <returns>BadRequest if some information is missing, Unauthorized if the user does not have permission to
    /// add an event to the campaign, Ok with the new event's Guid on success.</returns>
    [HttpPost("create-campaign-event/{campaignGuid:guid}")]
    public async Task<IActionResult> CreateCampaignEvent(Guid campaignGuid, [FromBody] CustomEvent newEvent)
    {
        try
        {
            // Check that the required fields are not null or empty.
            if (String.IsNullOrWhiteSpace(newEvent.EventName))
            {
                return BadRequest(FormatErrorMessage(EventNameIsRequired, CustomStatusCode.ValueNullOrEmpty));
            }

            if (newEvent.MaxAttendees is null or <= 0)
            {
                return BadRequest(FormatErrorMessage(MaxAttendeesNotNullOrZero, CustomStatusCode.ValueNullOrEmpty));
            }

            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                    campaignGuid, new Permission()
                    {
                        PermissionTarget = PermissionTargets.Events,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // Fill in the remaining required fields.
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            newEvent.EventCreatorId = userId.Value;
            newEvent.CampaignGuid = campaignGuid;
            newEvent.EventOf = null;

            newEvent.IsOpenJoin ??= false;

            var createdEvent = await _eventsService.AddEvent(newEvent);
            if (createdEvent.Item1 == CustomStatusCode.CampaignNotFound)
            {
                return BadRequest(FormatErrorMessage(CampaignNotFound, CustomStatusCode.CampaignNotFound));
            }

            // If the above if statement is false, then the event was created successfully.
            var eventGuid = createdEvent.Item3.Value;

            // No need to await this - all of its fail conditions can not happen, since the event was just created
            // and its Guid is 100% correct.
            // User is always added as a watcher to the events they create for a campaign.
            _eventsService.AddEventWatcher(userId.Value, eventGuid);

            return Ok(new { eventGuid });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating campaign event");
            return BadRequest("Error while creating campaign event");
        }
    }

    /// <summary>
    /// Updates an event for a campaign.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event to update.</param>
    /// <param name="campaignGuid">The Guid of the campaign.</param>
    /// <param name="updatedEvent">A <see cref="CustomEvent"/> object with the fields that the user wants to change filled in.</param>
    /// <param name="sendSms">Whether or not to send an SMS to update event participants and watchers of the changes.</param>
    /// <param name="sendEmail">Whether or not to send an email to update event participants and watchers of the changes.</param>
    /// <returns>Ok on success, Unauthorized if the user does not have permission to do this, BadRequest in case of bad data.</returns>
    [HttpPut("update-campaign-event/{campaignGuid:guid}/{eventGuid:guid}")]
    public async Task<IActionResult> UpdateEvent(Guid eventGuid, Guid campaignGuid, [FromBody] CustomEvent updatedEvent,
        [FromQuery] bool sendSms = false, [FromQuery] bool sendEmail = false)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                    campaignGuid, new Permission()
                    {
                        PermissionTarget = PermissionTargets.Events,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // Check that the event is in the campaign.
            if (!await EventsUtils.IsEventInCampaign(_eventsService, eventGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            updatedEvent.EventGuid = eventGuid;
            updatedEvent.CampaignGuid = campaignGuid;

            var updatedEventResult = await _eventsService.UpdateEvent(updatedEvent);

            // In case of a failure, return the appropriate error message.
            switch (updatedEventResult)
            {
                case CustomStatusCode.EventNotFound:
                    return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
                case CustomStatusCode.CampaignNotFound:
                    return BadRequest(FormatErrorMessage(CampaignNotFound, CustomStatusCode.CampaignNotFound));
            }


            // Otherwise, the event was updated successfully, and we check if we need to send messages to the participants.
            if (sendSms || sendEmail)
            {
                var (_, eventParticipants) = await _eventsService.GetEventParticipants(eventGuid);
                var (_, eventWatchers) = await _eventsService.GetEventWatchers(eventGuid);
                eventParticipants = eventParticipants.Concat(eventWatchers);

                var campaign = await _campaignsService.GetCampaignBasicInfo(campaignGuid);

                // If the event name was not updated, we need to get it from the DB.
                if (updatedEvent.EventName == null)
                {
                    var eventAfterUpdate = await _eventsService.GetEvent(eventGuid);
                    updatedEvent.EventName = eventAfterUpdate.EventName;
                }

                // Send messages to all participants and watchers.
                // The method is not awaited on purpose - there is no need to wait for the messages to be sent.
                foreach (var participant in eventParticipants)
                {
                    SendEventUpdatedMessage(
                        sendEmail,
                        sendSms,
                        participant.Email,
                        participant.PhoneNumber,
                        updatedEvent,
                        campaign.CampaignName
                    );
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating event");
            return BadRequest("Error while updating event");
        }
    }

    /// <summary>
    /// Updates a user's personal event.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event to update.</param>
    /// <param name="updatedEvent">A <see cref="CustomEvent"/> object with the fields the user wants to change filled in.</param>
    /// <param name="sendSms">Whether or not to send an sms to participants and watchers to inform them of the changes.</param>
    /// <param name="sendEmail">Whether or not to send an email to participants and watchers to inform them of the changes.</param>
    /// <returns>Ok if successful, BadRequest in case of bad data, Unauthorized if the user is not the creator of the event.</returns>
    [HttpPut("update-personal-event/{eventGuid:guid}")]
    public async Task<IActionResult> UpdatePersonalEvent(Guid eventGuid, [FromBody] CustomEvent updatedEvent,
        [FromQuery] bool sendSms = false, [FromQuery] bool sendEmail = false)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var creator = await _eventsService.GetEventCreatorUserId(eventGuid);
            if (creator == null)
            {
                return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
            }

            // Only the creator of the event can update it.
            if (creator.UserId != userId)
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            updatedEvent.EventGuid = eventGuid;
            updatedEvent.CampaignGuid = null;

            var updatedEventResult = await _eventsService.UpdateEvent(updatedEvent);

            // In case of a failure, return the appropriate error message.
            if (updatedEventResult == CustomStatusCode.EventNotFound)
            {
                return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
            }

            // Otherwise, the event was updated successfully, and we check if we need to send messages to the participants.
            if (sendSms || sendEmail)
            {
                var (_, eventParticipants) = await _eventsService.GetEventParticipants(eventGuid);
                var (_, eventWatchers) = await _eventsService.GetEventWatchers(eventGuid);
                eventParticipants = eventParticipants.Concat(eventWatchers);

                // Get the name of the user who updated the event.
                var sendingUser = await _usersService.GetUserPublicInfo(userId);
                string? name = sendingUser.FirstNameHeb != null
                    ? sendingUser.FirstNameHeb + " " + sendingUser.LastNameHeb
                    : sendingUser.DisplayNameEng;

                // If the event name was not updated, we need to get it from the DB.
                if (updatedEvent.EventName == null)
                {
                    var eventAfterUpdate = await _eventsService.GetEvent(eventGuid);
                    updatedEvent.EventName = eventAfterUpdate.EventName;
                }

                // Send messages to all participants and watchers.
                // The method is not awaited on purpose - there is no need to wait for the messages to be sent.
                foreach (var participant in eventParticipants)
                {
                    SendEventUpdatedMessage(
                        sendEmail,
                        sendSms,
                        participant.Email,
                        participant.PhoneNumber,
                        updatedEvent,
                        name
                    );
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating personal event");
            return BadRequest("Error while updating personal event");
        }
    }

    /// <summary>
    /// Deletes an event from a campaign.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event to delete.</param>
    /// <param name="campaignGuid">The Guid of the campaign the event belongs to.</param>
    /// <param name="sendSms">Whether to send an SMS to all related parties.</param>
    /// <param name="sendEmail">Whether to send an email to all related parties.</param>
    /// <returns>Ok on success, Unauthorized if the user does not have permission to do this, BadRequest in the case of bad data.</returns>
    [HttpDelete("delete-campaign-event/{campaignGuid:guid}/{eventGuid:guid}")]
    public async Task<IActionResult> DeleteEvent(Guid eventGuid, Guid campaignGuid,
        [FromQuery] bool sendSms = false, [FromQuery] bool sendEmail = false)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                    campaignGuid, new Permission()
                    {
                        PermissionTarget = PermissionTargets.Events,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            if (!await EventsUtils.IsEventInCampaign(_eventsService, eventGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // If we need to send emails or sms, we need to get the event participants and event info
            // before deleting the event.
            var eventParticipants = new List<UserPublicInfo>();
            var eventBackup = new EventWithCreatorDetails();
            if (sendEmail || sendSms)
            {
                eventBackup = await _eventsService.GetEvent(eventGuid);
                var res = await _eventsService.GetEventParticipants(eventGuid);
                eventParticipants = res.Item2.ToList();
                res = await _eventsService.GetEventWatchers(eventGuid);
                eventParticipants = eventParticipants.Concat(res.Item2).ToList();
            }

            var deletedEventResult = await _eventsService.DeleteEvent(eventGuid);

            switch (deletedEventResult)
            {
                case CustomStatusCode.EventNotFound:
                    return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
                case CustomStatusCode.CampaignNotFound:
                    return BadRequest(FormatErrorMessage(CampaignNotFound, CustomStatusCode.CampaignNotFound));
            }


            if (sendSms || sendEmail)
            {
                var campaign = await _campaignsService.GetCampaignBasicInfo(campaignGuid);

                foreach (var participant in eventParticipants)
                {
                    SendEventDeletedMessage(
                        sendEmail,
                        sendSms,
                        participant.Email,
                        participant.PhoneNumber,
                        eventBackup,
                        campaign.CampaignName
                    );
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting event");
            return BadRequest("Error while deleting event");
        }
    }

    /// <summary>
    /// Deletes a personal event.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event to delete.</param>
    /// <param name="sendSms">Whether to send an SMS to all related parties.</param>
    /// <param name="sendEmail">Whether to send an email to all related parties.</param>
    /// <returns>Ok on success, BadRequest in case of bad data, Unauthorized if the requesting user is not the event's creator.</returns>
    [HttpDelete("delete-personal-event/{eventGuid:guid}")]
    public async Task<IActionResult> DeletePersonalEvent(Guid eventGuid,
        [FromQuery] bool sendSms = false, [FromQuery] bool sendEmail = false)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var creator = await _eventsService.GetEventCreatorUserId(eventGuid);
            if (creator == null)
            {
                return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
            }

            if (creator.UserId != userId)
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // If we need to send emails or sms, we need to get the event participants and event info
            // before deleting the event.
            var eventParticipants = new List<UserPublicInfo>();
            var eventBackup = new EventWithCreatorDetails();
            if (sendEmail || sendSms)
            {
                eventBackup = await _eventsService.GetEvent(eventGuid);
                var res = await _eventsService.GetEventParticipants(eventGuid);
                eventParticipants = res.Item2.ToList();
                res = await _eventsService.GetEventWatchers(eventGuid);
                eventParticipants = eventParticipants.Concat(res.Item2).ToList();
            }

            var deletedEventResult = await _eventsService.DeleteEvent(eventGuid);

            if (deletedEventResult == CustomStatusCode.EventNotFound)
            {
                return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
            }

            if (sendSms || sendEmail)
            {
                var sendingUser = await _usersService.GetUserPublicInfo(userId);
                string? name = sendingUser.FirstNameHeb != null
                    ? sendingUser.FirstNameHeb + " " + sendingUser.LastNameHeb
                    : sendingUser.DisplayNameEng;

                foreach (var participant in eventParticipants)
                {
                    SendEventDeletedMessage(
                        sendEmail,
                        sendSms,
                        participant.Email,
                        participant.PhoneNumber,
                        eventBackup,
                        name
                    );
                }
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting personal event");
            return BadRequest("Error while deleting personal event");
        }
    }

    /// <summary>
    /// Adds the requesting user as a watcher of the event, giving them easy access to viewing the event in the app, and
    /// also to getting notifications about it.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event.</param>
    /// <returns>Ok on success, BadRequest in case of bad data (bad event Guid or already a watcher).</returns>
    [HttpPost("add-event-watcher/{eventGuid:guid}")]
    public async Task<IActionResult> AddEventWatcher(Guid eventGuid)
    {
        try
        {
            // Only the user themselves can join as a watcher to an event, so no need to check permissions,
            // as watchers are just users who have "easy access" to viewing the event.
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var addWatcherResult = await _eventsService.AddEventWatcher(userId.Value, eventGuid);

            return addWatcherResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                CustomStatusCode.DuplicateKey => BadRequest(FormatErrorMessage(AlreadyWatcher,
                    CustomStatusCode.DuplicateKey)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding event watcher");
            return BadRequest("Error while adding event watcher");
        }
    }

    /// <summary>
    /// Removes a user as a watcher of an event.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event</param>
    /// <returns>Ok on success, BadRequest in case of bad data (incorrect event guid).</returns>
    [HttpDelete("remove-event-watcher/{eventGuid:guid}")]
    public async Task<IActionResult> RemoveEventWatcher(Guid eventGuid)
    {
        try
        {
            // Only the user themselves can remove themselves as a watcher to an event, so no need to check permissions,
            // as watchers are just users who have "easy access" to viewing the event.
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var removeWatcherResult = await _eventsService.RemoveEventWatcher(userId.Value, eventGuid);

            return removeWatcherResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing event watcher");
            return BadRequest("Error while removing event watcher");
        }
    }

    /// <summary>
    /// Adds a user as a participant to a personal event.
    /// </summary>
    /// <param name="eventGuid">Guid of the event to add the user to.</param>
    /// <param name="userEmail">Email of the user to add to the event.</param>
    /// <param name="sendSms">Whether to send the user an Sms to inform him of being added to the event. Useful when the user
    /// was not added by themselves.</param>
    /// <param name="sendEmail">Like sms, but with an email.</param>
    /// <returns>Ok if the user was added successfully, BadRequest in case of bad data, Unauthorized if the user
    /// should not be able to add other users to that event.</returns>
    [HttpPost("add-personal-or-open-event-participant/{eventGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AddPersonalEventParticipant(Guid eventGuid, string userEmail,
        [FromQuery] bool sendSms = false, [FromQuery] bool sendEmail = false)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest(FormatErrorMessage(EmailRequired, CustomStatusCode.ValueNullOrEmpty));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var requestedEvent = await _eventsService.GetEvent(eventGuid);
            if (requestedEvent == null)
            {
                return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
            }

            // If the event is not open join, then only the creator can add participants.
            // If the event is open join, then anyone can add participants.
            if (requestedEvent.IsOpenJoin == false)
            {
                var creator = await _eventsService.GetEventCreatorUserId(eventGuid);
                if (creator == null)
                {
                    return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
                }

                if (creator.UserId != userId)
                {
                    return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                        CustomStatusCode.PermissionOrAuthorizationError));
                }
            }

            var addParticipantResult = await _eventsService.AddEventParticipant(eventGuid, userEmail: userEmail);

            switch (addParticipantResult)
            {
                case CustomStatusCode.EventNotFound:
                    return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
                case CustomStatusCode.EventAlreadyFull:
                    return BadRequest(FormatErrorMessage(EventAlreadyFull, CustomStatusCode.EventAlreadyFull));
                case CustomStatusCode.DuplicateKey:
                    return BadRequest(FormatErrorMessage(AlreadyParticipating, CustomStatusCode.DuplicateKey));
            }


            if (sendSms || sendEmail)
            {
                var sender = await _usersService.GetUserPublicInfo(userId);

                SendEventAssignedMessage(
                    sendEmail,
                    sendSms,
                    userEmail,
                    requestedEvent,
                    sender.FirstNameHeb + sender.LastNameHeb
                );
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding personal event participant");
            return BadRequest("Error while adding personal event participant");
        }
    }

    /// <summary>
    /// Adds a user as a participant to a campaign event.
    /// </summary>
    /// <param name="eventGuid">Guid of the event to add the user to.</param>
    /// <param name="campaignGuid">The Guid of the campaign the event belongs to.</param>
    /// <param name="userEmail">Email of the user to add to the event.</param>
    /// <param name="sendSms">Whether to send the user an Sms to inform him of being added to the event. Useful when the user
    /// was not added by themselves.</param>
    /// <param name="sendEmail">Like sms, but with an email.</param>
    /// <returns>Ok if the user was added successfully, BadRequest in case of bad data, Unauthorized if the user
    /// should not be able to add other users to that event.</returns>
    [HttpPost("add-campaign-event-participant/{campaignGuid:guid}/{eventGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AddCampaignEventParticipant(Guid eventGuid, Guid campaignGuid, string userEmail,
        [FromQuery] bool sendSms = false, [FromQuery] bool sendEmail = false)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest(FormatErrorMessage(EmailRequired, CustomStatusCode.ValueNullOrEmpty));
            }

            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                    campaignGuid, new Permission()
                    {
                        PermissionTarget = PermissionTargets.Events,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // Check if the event is in the campaign.
            if (!await EventsUtils.IsEventInCampaign(_eventsService, eventGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var addParticipantResult = await _eventsService.AddEventParticipant(eventGuid, userEmail: userEmail);

            // In case of an error, return the error.
            switch (addParticipantResult)
            {
                case CustomStatusCode.EventNotFound:
                    return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
                case CustomStatusCode.EventAlreadyFull:
                    return BadRequest(FormatErrorMessage(EventAlreadyFull, CustomStatusCode.EventAlreadyFull));
                case CustomStatusCode.DuplicateKey:
                    return BadRequest(FormatErrorMessage(AlreadyParticipating, CustomStatusCode.DuplicateKey));
            }


            // Else, send the message if needed.
            if (sendSms || sendEmail)
            {
                var requestedEvent = await _eventsService.GetEvent(eventGuid);
                var sendingCampaign = await _campaignsService.GetCampaignBasicInfo(campaignGuid);

                SendEventAssignedMessage(
                    sendEmail,
                    sendSms,
                    userEmail,
                    requestedEvent,
                    sendingCampaign.CampaignName
                );
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding campaign event participant");
            return BadRequest("Error while adding campaign event participant");
        }
    }

    /// <summary>
    /// Removes a user from being a participant in a personal event.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event.</param>
    /// <param name="userEmail">The email of the user to remove.</param>
    /// <returns>BadRequest in case of bad data, Unauthorized if the user should not be able to remove the participant
    /// from this event, Ok on success.</returns>
    [HttpDelete("remove-event-participant-personal-event/{eventGuid:guid}/{userEmail}")]
    public async Task<IActionResult> RemoveEventParticipantPersonalEvent(Guid eventGuid, string userEmail)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest(FormatErrorMessage(EmailRequired, CustomStatusCode.ValueNullOrEmpty));
            }

            // Only the creator of the event can remove participants from a personal event.
            // Only exception is if the user is removing themselves from the event.
            var creator = await _eventsService.GetEventCreatorUserId(eventGuid);
            if (creator == null)
            {
                return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            // If the user is not the creator of the event, then they must be the user they are trying to remove.
            if (creator.UserId != userId)
            {
                var requestingUser = await _usersService.GetUserByEmail(userEmail);
                if (requestingUser == null)
                {
                    return BadRequest(FormatErrorMessage(UserNotFound, CustomStatusCode.UserNotFound));
                }

                // Check to make sure the user they are trying to remove is the same as the user making the request.
                if (requestingUser.UserId != userId)
                {
                    return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                        CustomStatusCode.PermissionOrAuthorizationError));
                }
            }

            // If all the above checks pass, then the user is authorized to remove the participant.
            var removeParticipantResult = await _eventsService.RemoveEventParticipant(eventGuid, userEmail: userEmail);

            return removeParticipantResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                CustomStatusCode.UserNotFound => BadRequest(FormatErrorMessage(UserNotFound,
                    CustomStatusCode.UserNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing event participant from personal event");
            return BadRequest("Error while removing event participant from personal event");
        }
    }

    /// <summary>
    /// Removes a user from being a participant in a campaign event.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event.</param>
    /// <param name="campaignGuid">The Guid of the campaign the event belongs to.</param>
    /// <param name="userEmail">The email of the user to remove.</param>
    /// <returns>BadRequest in case of bad data, Unauthorized if the user should not be able to remove the participant
    /// from this event, Ok on success.</returns>
    [HttpDelete("remove-event-participant-campaign-event/{campaignGuid:guid}/{eventGuid:guid}/{userEmail}")]
    public async Task<IActionResult> RemoveEventParticipantCampaignEvent(Guid eventGuid, Guid campaignGuid,
        string userEmail)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest(FormatErrorMessage(EmailRequired, CustomStatusCode.ValueNullOrEmpty));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Events,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                // If the user does not have permission to edit the event, then they must be the user they are trying to remove.
                var requestingUser = await _usersService.GetUserByEmail(userEmail);
                if (requestingUser == null)
                {
                    return BadRequest(FormatErrorMessage(UserNotFound, CustomStatusCode.UserNotFound));
                }

                // Check to make sure the user they are trying to remove is the same as the user making the request.
                // If not, then they are not authorized to remove the participant.
                if (requestingUser.UserId != userId)
                {
                    return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                        CustomStatusCode.PermissionOrAuthorizationError));
                }
            }

            if (!await EventsUtils.IsEventInCampaign(_eventsService, eventGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }


            // If all the above checks pass, then the user is authorized to remove the participant.
            var removeParticipantResult = await _eventsService.RemoveEventParticipant(eventGuid, userEmail: userEmail);

            return removeParticipantResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                CustomStatusCode.UserNotFound => BadRequest(FormatErrorMessage(UserNotFound,
                    CustomStatusCode.UserNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing event participant from campaign event");
            return BadRequest("Error while removing event participant from campaign event");
        }
    }

    /// <summary>
    /// Gets the list of participants for a personal event.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event.</param>
    /// <returns>BadRequest in case of bad eventGuid, Unauthorized if the requesting user is not the creator,
    /// Ok with a list of <see cref="UserPublicInfo"/> objects on success.</returns>
    [HttpGet("get-event-participants-personal-event/{eventGuid:guid}")]
    public async Task<IActionResult> GetEventParticipantsPersonalEvent(Guid eventGuid)
    {
        try
        {
            // Only the creator of the event can get the participants from a personal event.
            var creator = await _eventsService.GetEventCreatorUserId(eventGuid);
            if (creator == null)
            {
                return BadRequest(FormatErrorMessage(EventNotFound, CustomStatusCode.EventNotFound));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            // If the user is not the creator of the event, then they are not authorized to get the participants.
            if (creator.UserId != userId)
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // If all the above checks pass, then the user is authorized to get the participants.
            var (_, participants) = await _eventsService.GetEventParticipants(eventGuid);

            return Ok(participants);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting event participants from personal event");
            return BadRequest("Error while getting event participants from personal event");
        }
    }

    /// <summary>
    /// Gets the list of participants for a campaign event.
    /// </summary>
    /// <param name="eventGuid">The Guid of the event.</param>
    /// <param name="campaignGuid">The Guid of the campaign the event belongs to.</param>
    /// <returns>BadRequest in case of bad eventGuid, Unauthorized if the requesting user is not the creator,
    /// Ok with a list of <see cref="UserPublicInfo"/> objects on success.</returns>
    /// <returns></returns>
    [HttpGet("get-event-participants-campaign-event/{campaignGuid:guid}/{eventGuid:guid}")]
    public async Task<IActionResult> GetEventParticipantsCampaignEvent(Guid eventGuid, Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Events,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // If the user has permission to edit the campaign's events, verify that the event is actually in the campaign.
            if (!await EventsUtils.IsEventInCampaign(_eventsService, eventGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // If all the above checks pass, then the user is authorized to get the participants.
            var (_, participants) = await _eventsService.GetEventParticipants(eventGuid);

            return Ok(participants);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting event participants from campaign event");
            return BadRequest("Error while getting event participants from campaign event");
        }
    }

    /// <summary>
    /// Allows a user to add an event for a user they manage.
    /// </summary>
    /// <param name="userEmail">Email of the managed user.</param>
    /// <param name="newEvent">An instance of <see cref="CustomEvent"/> containing the parameters for creating an event.</param>
    /// <param name="sendSms">Whether to inform the managed user via SMS.</param>
    /// <param name="sendEmail">Whether to inform the managed user via email.</param>
    /// <returns>Ok on success, Unauthorized if the requesting user is not a manager of the other user,
    /// BadRequest if the event is missing a name or max attendees value.</returns>
    [HttpPost("add-event-for-managed-user/{userEmail}/{sendSms:bool}/{sendEmail:bool}")]
    public async Task<IActionResult> AddEventForManagedUser(string userEmail,
        [FromBody] CustomEvent newEvent, [FromQuery] bool sendSms = false, [FromQuery] bool sendEmail = false)
    {
        try
        {
            // Check that the new event is valid.
            if (string.IsNullOrWhiteSpace(newEvent.EventName))
            {
                return BadRequest(FormatErrorMessage(EventNameIsRequired, CustomStatusCode.IllegalValue));
            }

            if (newEvent.MaxAttendees is null or <= 0)
            {
                return BadRequest(FormatErrorMessage(MaxAttendeesNotNullOrZero, CustomStatusCode.IllegalValue));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            // Check to make sure the user is authorized to add an event for the managed user.
            var managedUsers = await _scheduleManagersService.GetManagedUsers(userId.Value);

            // If the user is not managing the user they are trying to add an event for, then they are not authorized.
            var managedUser = managedUsers.FirstOrDefault(u => u.Email == userEmail);
            if (managedUser == null)
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // Get the user id of the managed user.
            var managedUserWithId = await _usersService.GetUserByEmail(userEmail);

            newEvent.EventOf = managedUserWithId.UserId;
            newEvent.EventCreatorId = userId.Value;
            newEvent.CampaignId = null;

            // Else, the user is authorized to add an event for the managed user.
            var (_, _, eventGuid) = await _eventsService.AddEvent(newEvent);

            _eventsService.AddEventParticipant(eventGuid.Value, userId: userId.Value);
            _eventsService.AddEventWatcher(userId.Value, eventGuid.Value);

            // If the user wants to send an email or sms, then send it.
            if (sendSms || sendEmail)
            {
                var eventCreator = await _usersService.GetUserPublicInfo(userId.Value);
                string? senderName = eventCreator.FirstNameHeb != null
                    ? eventCreator.FirstNameHeb + " " + eventCreator.LastNameHeb
                    : eventCreator.DisplayNameEng;

                SendEventCreatedForUserMessage(sendSms, sendEmail, userEmail, newEvent,
                    senderName, senderName);
            }


            return Ok(new { eventGuid });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding event for managed user");
            return BadRequest("Error while adding event for managed user");
        }
    }

    /// <summary>
    /// Gets a list of all of the user's personal events.
    /// </summary>
    /// <returns>Ok with a list of all of the user's personal events.</returns>
    [HttpGet("get-personal-events")]
    public async Task<IActionResult> GetPersonalEvents()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            var events = await _eventsService.GetPersonalEvents(userId.Value);

            return Ok(events);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting personal events");
            return BadRequest("Error while getting personal events");
        }
    }

    /// <summary>
    /// Gets a list of all the events of a user that the requesting user manages.
    /// </summary>
    /// <param name="userEmail">Email of the managed user.</param>
    /// <returns>Unauthorized if the requesting user does not manage the other user, Ok with the list of events otherwise,
    /// with the events being instances of <see cref="GetUserEventsResult"/>.</returns>
    [HttpGet("get-managed-user-events/{userEmail}")]
    public async Task<IActionResult> GetManagedUserEvents(string userEmail)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var managedUsers = await _scheduleManagersService.GetManagedUsers(userId.Value);

            // If the user is not managing the user they are trying to add an event for, then they are not authorized.
            var managedUser = managedUsers.FirstOrDefault(u => u.Email == userEmail);
            if (managedUser == null)
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            // Get the user id of the managed user.
            var managedUserWithId = await _usersService.GetUserByEmail(userEmail);

            var events = await _eventsService.GetUserEvents(managedUserWithId.UserId);

            return Ok(events);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting managed user events");
            return BadRequest("Error while getting managed user events");
        }
    }
}