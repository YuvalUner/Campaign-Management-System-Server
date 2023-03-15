using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for the public board.
/// Provides a web API and service policy for <see cref="IPublicBoardService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PublicBoardController: Controller
{
    private readonly IPublicBoardService _publicBoardService;
    private readonly ILogger<PublicBoardController> _logger;
    
    public PublicBoardController(IPublicBoardService publicBoardService, ILogger<PublicBoardController> logger)
    {
        _publicBoardService = publicBoardService;
        _logger = logger;
    }
    
    /// <summary>
    /// Gets a personalized public board - events and announcements - for the user.
    /// </summary>
    /// <param name="limit">How many to get of each.</param>
    /// <param name="offset">In case this is not the first request in the session, send a value greater than 0
    /// to get the next x results.</param>
    /// <returns></returns>
    [HttpGet("public-board")]
    public async Task<IActionResult> GetPersonalizedPublicBoard([FromQuery] int? limit, [FromQuery] int? offset)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            if (offset is null or < 0)
            {
                offset = 0;
            }
            
            var events = await _publicBoardService.GetEventsForUser(userId, limit, offset);
            var announcements = await _publicBoardService.GetAnnouncementsForUser(userId, limit, offset);
            
            return Ok(new {events, announcements});
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting campaign published events");
            return StatusCode(500, "Error while getting campaign published events");
        }
    }

    [HttpGet("events-search")]
    public async Task<IActionResult> SearchPublicEvents([FromQuery] EventsSearchParams searchParams)
    {
        try
        {
            var events = await _publicBoardService.SearchEvents(searchParams);
            return Ok(events);   
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while searching public events");
            return StatusCode(500, "Error while searching public events");
        }
    }
    
    [HttpGet("announcements-search")]
    public async Task<IActionResult> SearchPublicAnnouncements([FromQuery] AnnouncementsSearchParams searchParams)
    {
        try
        {
            var announcements = await _publicBoardService.SearchAnnouncements(searchParams);
            return Ok(announcements);   
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while searching public announcements");
            return StatusCode(500, "Error while searching public announcements");
        }
    }
    
    [Authorize]
    [HttpPost("subscribe-for-notifications/{campaignGuid:guid}")]
    public async Task<IActionResult> SubscribeForNotifications(Guid campaignGuid, [FromBody] NotificationUponPublishSettings settings)
    {
        try
        {
            if (settings is {ViaSms: false, ViaEmail: false} or {ViaSms: null, ViaEmail: null})
            {
                return BadRequest(FormatErrorMessage(NotificationSettingsRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            settings.UserId = userId;
            settings.CampaignGuid = campaignGuid;
            var statusCode = await _publicBoardService.AddNotificationSettings(settings);
            return statusCode switch
            {
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound, statusCode)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while subscribing for notifications");
            return StatusCode(500, "Error while subscribing for notifications");
        }
    }
    
    [Authorize]
    [HttpPut("update-notification-settings/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateNotificationSettings(Guid campaignGuid, [FromBody] NotificationUponPublishSettings settings)
    {
        try
        {
            if (settings is {ViaSms: false, ViaEmail: false} or {ViaSms: null, ViaEmail: null})
            {
                return BadRequest(FormatErrorMessage(NotificationSettingsRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            settings.UserId = userId;
            settings.CampaignGuid = campaignGuid;
            var statusCode = await _publicBoardService.UpdateNotificationSettings(settings);
            return statusCode switch
            {
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound, statusCode)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating notification settings");
            return StatusCode(500, "Error while updating notification settings");
        }
    }
    
    [Authorize]
    [HttpDelete("unsubscribe-from-notifications/{campaignGuid:guid}")]
    public async Task<IActionResult> UnsubscribeFromNotifications(Guid campaignGuid)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var settings = new NotificationUponPublishSettings()
            {
                UserId = userId,
                CampaignGuid = campaignGuid
            };
            
            var statusCode = await _publicBoardService.RemoveNotificationSettings(settings);
            return statusCode switch
            {
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound, statusCode)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while unsubscribing from notifications");
            return StatusCode(500, "Error while unsubscribing from notifications");
        }
    }

    [Authorize]
    [HttpGet("notification-settings")]
    public async Task<IActionResult> GetNotificationSettings()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            var settings = await _publicBoardService.GetNotificationSettingsForUser(userId.Value);
            return Ok(settings);   
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting notification settings");
            return StatusCode(500, "Error while getting notification settings");
        }
    }
    
}