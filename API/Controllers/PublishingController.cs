using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PublishingController : Controller
{
    private readonly IPublishingService _publishingService;
    private readonly ILogger<PublishingController> _logger;
    private readonly int _maxAnnouncementTitleLength = 100;
    private readonly int _maxAnnouncementContentLength = 4000;
    private readonly ICampaignsService _campaignsService;
    private readonly ISmsMessageSendingService _smsMessageSendingService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly IPublicBoardService _publicBoardService;
    private readonly IEventsService _eventsService;

    public PublishingController(IPublishingService publishingService, ILogger<PublishingController> logger,
        ICampaignsService campaignsService, ISmsMessageSendingService smsMessageSendingService,
        IEmailSendingService emailSendingService, IPublicBoardService publicBoardService, IEventsService eventsService)
    {
        _publishingService = publishingService;
        _logger = logger;
        _campaignsService = campaignsService;
        _smsMessageSendingService = smsMessageSendingService;
        _emailSendingService = emailSendingService;
        _publicBoardService = publicBoardService;
        _eventsService = eventsService;
    }

    private async Task SendEventPublishedNotification(NotificationUponPublishSettingsForCampaign settings,
        EventWithCreatorDetails eventInfo, string? senderName)
    {
        if (settings.ViaEmail == true)
        {
            await _emailSendingService.SendEventPublishedEmailAsync(
                eventInfo.EventName,
                eventInfo.EventLocation,
                eventInfo.EventStartTime,
                eventInfo.EventEndTime,
                settings.Email,
                senderName
            );
        }
        
        if (settings.ViaSms == true)
        {
            await _smsMessageSendingService.SendEventPublishedMessageAsync(
                eventInfo.EventName,
                eventInfo.EventLocation,
                eventInfo.EventStartTime,
                eventInfo.EventEndTime,
                settings.PhoneNumber,
                senderName,
                CountryCodes.Israel
            );
        }
    }

    private async Task SendAnnouncementPublishedNotification(NotificationUponPublishSettingsForCampaign settings,
        Announcement announcement, string? senderName)
    {
        if (settings.ViaEmail == true)
        {
            await _emailSendingService.SendAnnouncementPublishedEmailAsync(
                announcement.AnnouncementTitle,
                announcement.AnnouncementContent,
                settings.Email,
                senderName
            );
        }

        if (settings.ViaSms == true)
        {
            await _smsMessageSendingService.SendAnnouncementPublishedMessageAsync(
                announcement.AnnouncementTitle,
                settings.PhoneNumber,
                senderName,
                CountryCodes.Israel
            );
        }
    }
    
    [HttpPost("publish-event/{eventGuid:guid}/{campaignGuid:guid}")]
    public async Task<IActionResult> PublishEvent(Guid eventGuid, Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                    campaignGuid, new Permission()
                    {
                        PermissionTarget = PermissionTargets.Publishing,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var result = await _publishingService.PublishEvent(eventGuid, userId);

            switch (result)
            {
                case CustomStatusCode.EventNotFound:
                    return NotFound(FormatErrorMessage(EventNotFound, result));
                case CustomStatusCode.DuplicateKey:
                    return BadRequest(FormatErrorMessage(EventAlreadyPublished, result));
                case CustomStatusCode.IncorrectEventType:
                    return BadRequest(FormatErrorMessage(EventNotAssociatedToCampaign, result));
            };

            var campaignInfo = await _campaignsService.GetCampaignBasicInfo(campaignGuid);
            var userSettings = await _publicBoardService.GetNotificationSettingsForCampaign(campaignGuid);
            var eventInfo = await _eventsService.GetEvent(eventGuid);

            // Send notifications to all users who have subscribed to notifications for this campaign
            foreach (var settings in userSettings)
            {
                SendEventPublishedNotification(settings, eventInfo, campaignInfo.CampaignName);
            }
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error publishing event");
            return StatusCode(500, "Error publishing event");
        }
    }
    
    [HttpPost("unpublish-event/{eventGuid:guid}/{campaignGuid:guid}")]
    public async Task<IActionResult> UnpublishEvent(Guid eventGuid, Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                campaignGuid, new Permission()
                {
                    PermissionTarget = PermissionTargets.Publishing,
                    PermissionType = PermissionTypes.Edit
                }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var result = await _publishingService.UnpublishEvent(eventGuid);

            return result switch
            {
                CustomStatusCode.EventNotFound => NotFound(FormatErrorMessage(EventNotFound, result)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error unpublishing event");
            return StatusCode(500, "Error unpublishing event");
        }
    }
    
    [HttpGet("campaign-published-events-campaign-member/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignPublishedEventsCampaignMember(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                campaignGuid, new Permission()
                {
                    PermissionTarget = PermissionTargets.Publishing,
                    PermissionType = PermissionTypes.View
                }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var (statusCode, result) = await _publishingService.GetCampaignPublishedEvents(campaignGuid);

            return statusCode switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                _ => Ok(result)
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting published events for campaign");
            return StatusCode(500, "Error getting published events for campaign");
        }
    }

    /// <summary>
    /// A general version of the above method, which does not return the publisher info.
    /// This is used for members of the general public, who should not be able to see the publisher info (except their name).
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    [HttpGet("campaign-published-events-general/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignPublishedEventsGeneral(Guid campaignGuid)
    {
        try
        {
            var (statusCode, result) = await _publishingService.GetCampaignPublishedEvents(campaignGuid);
            
            if (statusCode == CustomStatusCode.CampaignNotFound)
            {
                return NotFound(FormatErrorMessage(CampaignNotFound, statusCode));
            }

            var resultWithoutPublisherInfo = result.Select(x => new
            {
                x.EventName,
                x.EventDescription,
                x.EventStartTime,
                x.EventEndTime,
                x.EventLocation,
                x.MaxAttendees,
                x.PublishingDate,
                x.EventGuid,
                x.CampaignName,
                x.CampaignGuid,
                x.CampaignLogoUrl,
                x.FirstNameHeb,
                x.LastNameHeb,
            });
            
            return Ok(resultWithoutPublisherInfo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting published events for campaign");
            return StatusCode(500, "Error getting published events for campaign");
        }
    }
    
    [HttpPost("publish-announcement/{campaignGuid:guid}")]
    public async Task<IActionResult> PublishAnnouncement(Guid campaignGuid, [FromBody] Announcement announcement)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                campaignGuid, new Permission()
                {
                    PermissionTarget = PermissionTargets.Publishing,
                    PermissionType = PermissionTypes.Edit
                }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            // Validate the announcement - must have a title and content, and the title and content must be within the max length
            if (string.IsNullOrWhiteSpace(announcement.AnnouncementTitle))
            {
                return BadRequest(FormatErrorMessage(AnnouncementTitleRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            if (string.IsNullOrWhiteSpace(announcement.AnnouncementContent))
            {
                return BadRequest(FormatErrorMessage(AnnouncementContentRequired, CustomStatusCode.ValueNullOrEmpty));
            }
            
            if (announcement.AnnouncementTitle.Length > _maxAnnouncementTitleLength)
            {
                return BadRequest(FormatErrorMessage(AnnouncementTitleTooLong, CustomStatusCode.IllegalValue));
            }
            
            if (announcement.AnnouncementContent.Length > _maxAnnouncementContentLength)
            {
                return BadRequest(FormatErrorMessage(AnnouncementContentTooLong, CustomStatusCode.IllegalValue));
            }

            announcement.PublisherId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var (statusCode, newAnnouncementGuid) = await _publishingService.PublishAnnouncement(announcement, campaignGuid);

            switch (statusCode)
            {
                case CustomStatusCode.CampaignNotFound:
                    return NotFound(FormatErrorMessage(CampaignNotFound, statusCode));
                case CustomStatusCode.UserNotFound:
                    return NotFound(FormatErrorMessage(UserNotFound, statusCode));
            };

            var campaignInfo = await _campaignsService.GetCampaignBasicInfo(campaignGuid);
            var userSettings = await _publicBoardService.GetNotificationSettingsForCampaign(campaignGuid);
            
            // Send a notification to all users who have the "Receive notifications for new announcements" setting enabled
            foreach (var settings in userSettings)
            {
                SendAnnouncementPublishedNotification(settings, announcement, campaignInfo.CampaignName);
            }
            
            return Ok(new {newAnnouncementGuid});
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error publishing announcement");
            return StatusCode(500, "Error publishing announcement");
        }
    }
    
    [HttpPost("unpublish-announcement/{announcementGuid:guid}/{campaignGuid:guid}")]
    public async Task<IActionResult> UnpublishAnnouncement(Guid announcementGuid, Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                campaignGuid, new Permission()
                {
                    PermissionTarget = PermissionTargets.Publishing,
                    PermissionType = PermissionTypes.Edit
                }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var result = await _publishingService.UnpublishAnnouncement(announcementGuid);

            return result switch
            {
                CustomStatusCode.AnnouncementNotFound => NotFound(FormatErrorMessage(AnnouncementNotFound, result)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error unpublishing announcement");
            return StatusCode(500, "Error unpublishing announcement");
        }
    }
    
    [HttpGet("campaign-published-announcements-campaign-member/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignPublishedAnnouncementsCampaignMember(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext,
                campaignGuid, new Permission()
                {
                    PermissionTarget = PermissionTargets.Publishing,
                    PermissionType = PermissionTypes.View
                }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var (statusCode, result) = await _publishingService.GetCampaignAnnouncements(campaignGuid);

            return statusCode switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                _ => Ok(result)
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting published announcements for campaign");
            return StatusCode(500, "Error getting published announcements for campaign");
        }
    }
    
    /// <summary>
    /// A general version of the above method, which does not return the publisher info.
    /// This is used for members of the general public, who should not be able to see the publisher info (except their name).
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    [HttpGet("campaign-published-announcements-general/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCampaignPublishedAnnouncementsGeneral(Guid campaignGuid)
    {
        try
        {
            var (statusCode, result) = await _publishingService.GetCampaignAnnouncements(campaignGuid);
            
            if (statusCode == CustomStatusCode.CampaignNotFound)
            {
                return NotFound(FormatErrorMessage(CampaignNotFound, statusCode));
            }

            var resultWithoutPublisherInfo = result.Select(x => new
            {
                x.AnnouncementTitle,
                x.AnnouncementContent,
                x.PublishingDate,
                x.AnnouncementGuid,
                x.CampaignName,
                x.CampaignGuid,
                x.CampaignLogoUrl,
                x.FirstNameHeb,
                x.LastNameHeb
            });
            
            return Ok(resultWithoutPublisherInfo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting published announcements for campaign");
            return StatusCode(500, "Error getting published announcements for campaign");
        }
    }
}