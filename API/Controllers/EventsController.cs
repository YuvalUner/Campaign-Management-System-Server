using System.Reflection.Metadata;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : Controller
{
    private readonly IEventsService _eventsService;
    private readonly ILogger<EventsController> _logger;
    private readonly IUsersService _usersService;
    private readonly IScheduleManagersService _scheduleManagersService;

    public EventsController(IEventsService eventsService, ILogger<EventsController> logger, IUsersService usersService,
        IScheduleManagersService scheduleManagersService)
    {
        _eventsService = eventsService;
        _logger = logger;
        _usersService = usersService;
        _scheduleManagersService = scheduleManagersService;
    }

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
            return BadRequest("Error while getting campaign events");
        }
    }

    [HttpPost("create-personal-event")]
    public async Task<IActionResult> CreatePersonalEvent([FromBody] CustomEvent newEvent)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(newEvent.EventName))
            {
                return BadRequest(FormatErrorMessage(EventNameIsRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            if (newEvent.MaxAttendees == null || newEvent.MaxAttendees <= 0)
            {
                return BadRequest(FormatErrorMessage(MaxAttendeesNotNullOrZero, CustomStatusCode.ValueNullOrEmpty));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            newEvent.EventCreatorId = userId.Value;
            newEvent.CampaignGuid = null;
            newEvent.EventOf = userId.Value;
            
            // If the user did not specify whether the event is open join, then it is not.
            if (newEvent.IsOpenJoin == null)
            {
                newEvent.IsOpenJoin = false;
            }

            // The event is always created successfully, so long as the event name is not empty and campaignGuid is null.
            var (statusCode, eventId, eventGuid) = await _eventsService.AddEvent(newEvent);
            
            // No need to await this - all of its fail conditions can not happen, since the event was just created
            // and its Guid is 100% correct.
            // User is always added as a participant to their own event.
            _eventsService.AddEventParticipant(eventGuid.Value, userId.Value);

            return Ok(eventGuid);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating personal event");
            return BadRequest("Error while creating personal event");
        }
    }

    [HttpPost("create-campaign-event/{campaignGuid:guid}")]
    public async Task<IActionResult> CreateCampaignEvent(Guid campaignGuid, [FromBody] CustomEvent newEvent)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(newEvent.EventName))
            {
                return BadRequest(FormatErrorMessage(EventNameIsRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            if (newEvent.MaxAttendees == null || newEvent.MaxAttendees <= 0)
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

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            newEvent.EventCreatorId = userId.Value;
            newEvent.CampaignGuid = campaignGuid;
            newEvent.EventOf = null;
            
            if (newEvent.IsOpenJoin == null)
            {
                newEvent.IsOpenJoin = false;
            }

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

            return Ok(eventGuid);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating campaign event");
            return BadRequest("Error while creating campaign event");
        }
    }

    [HttpPut("update-campaign-event/{campaignGuid:guid}/{eventGuid:guid}")]
    public async Task<IActionResult> UpdateEvent(Guid eventGuid, Guid campaignGuid, [FromBody] CustomEvent updatedEvent)
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

            updatedEvent.EventGuid = eventGuid;

            var updatedEventResult = await _eventsService.UpdateEvent(updatedEvent);

            return updatedEventResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                CustomStatusCode.CampaignNotFound => BadRequest(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating event");
            return BadRequest("Error while updating event");
        }
    }

    [HttpPut("update-personal-event/{eventGuid:guid}")]
    public async Task<IActionResult> UpdatePersonalEvent(Guid eventGuid, [FromBody] CustomEvent updatedEvent)
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

            updatedEvent.EventGuid = eventGuid;
            updatedEvent.CampaignGuid = null;

            var updatedEventResult = await _eventsService.UpdateEvent(updatedEvent);

            return updatedEventResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating personal event");
            return BadRequest("Error while updating personal event");
        }
    }

    [HttpDelete("delete-campaign-event/{campaignGuid:guid}/{eventGuid:guid}")]
    public async Task<IActionResult> DeleteEvent(Guid eventGuid, Guid campaignGuid)
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

            var deletedEventResult = await _eventsService.DeleteEvent(eventGuid);

            return deletedEventResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                CustomStatusCode.CampaignNotFound => BadRequest(FormatErrorMessage(CampaignNotFound,
                    CustomStatusCode.CampaignNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting event");
            return BadRequest("Error while deleting event");
        }
    }

    [HttpDelete("delete-personal-event/{eventGuid:guid}")]
    public async Task<IActionResult> DeletePersonalEvent(Guid eventGuid)
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

            var deletedEventResult = await _eventsService.DeleteEvent(eventGuid);

            return deletedEventResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting personal event");
            return BadRequest("Error while deleting personal event");
        }
    }

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

    [HttpPost("add-personal-or-open-event-participant/{eventGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AddPersonalEventParticipant(Guid eventGuid, string userEmail)
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
            if (!requestedEvent.IsOpenJoin)
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

            return addParticipantResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                CustomStatusCode.EventAlreadyFull => BadRequest(FormatErrorMessage(EventAlreadyFull,
                    CustomStatusCode.EventAlreadyFull)),
                CustomStatusCode.DuplicateKey => BadRequest(FormatErrorMessage(AlreadyParticipating,
                    CustomStatusCode.DuplicateKey)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding personal event participant");
            return BadRequest("Error while adding personal event participant");
        }
    }

    [HttpPost("add-campaign-event-participant/{campaignGuid:guid}/{eventGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AddCampaignEventParticipant(Guid eventGuid, Guid campaignGuid, string userEmail)
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
            
            if (!await EventsUtils.IsEventInCampaign(_eventsService, eventGuid, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var addParticipantResult = await _eventsService.AddEventParticipant(eventGuid, userEmail: userEmail);

            return addParticipantResult switch
            {
                CustomStatusCode.EventNotFound => BadRequest(FormatErrorMessage(EventNotFound,
                    CustomStatusCode.EventNotFound)),
                CustomStatusCode.EventAlreadyFull => BadRequest(FormatErrorMessage(EventAlreadyFull,
                    CustomStatusCode.EventAlreadyFull)),
                CustomStatusCode.DuplicateKey => BadRequest(FormatErrorMessage(AlreadyParticipating,
                    CustomStatusCode.DuplicateKey)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding campaign event participant");
            return BadRequest("Error while adding campaign event participant");
        }
    }

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
            var participants = await _eventsService.GetEventParticipants(eventGuid);

            return Ok(participants.Item2);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting event participants from personal event");
            return BadRequest("Error while getting event participants from personal event");
        }
    }

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
            var participants = await _eventsService.GetEventParticipants(eventGuid);

            return Ok(participants.Item2);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting event participants from campaign event");
            return BadRequest("Error while getting event participants from campaign event");
        }
    }

    [HttpPost("add-event-for-managed-user/{userEmail}")]
    public async Task<IActionResult> AddEventForManagedUser(string userEmail, [FromBody] CustomEvent newEvent)
    {
        try
        {
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
            var (statusCode, eventId, eventGuid) = await _eventsService.AddEvent(newEvent);

            return Ok(eventGuid);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding event for managed user");
            return BadRequest("Error while adding event for managed user");
        }
    }

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
    
}