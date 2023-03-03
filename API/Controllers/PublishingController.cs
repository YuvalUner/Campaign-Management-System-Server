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
public class PublishingController : Controller
{
    private readonly IPublishingService _publishingService;
    private readonly ILogger<PublishingController> _logger;
    
    public PublishingController(IPublishingService publishingService, ILogger<PublishingController> logger)
    {
        _publishingService = publishingService;
        _logger = logger;
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

            return result switch
            {
                CustomStatusCode.EventNotFound => NotFound(FormatErrorMessage(EventNotFound, result)),
                CustomStatusCode.DuplicateKey => BadRequest(FormatErrorMessage(EventAlreadyPublished, result)),
                CustomStatusCode.IncorrectEventType => BadRequest(FormatErrorMessage(EventNotAssociatedToCampaign,
                    result)),
                CustomStatusCode.UserNotFound => NotFound(FormatErrorMessage(UserNotFound, result)),
                _ => Ok()
            };
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
    /// This is used for members of the general public, who should not be able to see the publisher info.
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
                x.CampaignLogoUrl
            });
            
            return Ok(resultWithoutPublisherInfo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting published events for campaign");
            return StatusCode(500, "Error getting published events for campaign");
        }
    }
}