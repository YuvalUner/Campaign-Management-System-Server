using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for handling notifications settings for users.
/// Provides a web API and service policy for <see cref="INotificationsService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationsService _notificationsService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationsService notificationsService, ILogger<NotificationsController> logger)
    {
        _notificationsService = notificationsService;
        _logger = logger;
    }

    /// <summary>
    /// Adds a user to a campaign's list of users to notify when a new user joins the campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the related campaign.</param>
    /// <param name="notificationSettings">An instance of <see cref="NotificationSettings"/> with the settings the user
    /// wants.</param>
    /// <returns>Unauthorized if the user does not have permission to view the campaign's users list, Ok otherwise.</returns>
    [HttpPost("add/{campaignGuid:guid}")]
    public async Task<IActionResult> AddUserToNotify(Guid campaignGuid, [FromBody] NotificationSettings notificationSettings)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                new Permission()
                {
                    PermissionTarget = PermissionTargets.CampaignUsersList,
                    PermissionType = PermissionTypes.View
                }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            await _notificationsService.AddUserToNotify(userId, campaignGuid, notificationSettings.ViaSms, notificationSettings.ViaEmail);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding user to notify");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Removes a user from a campaign's list of users to notify when a new user joins the campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the related campaign.</param>
    /// <returns>Unauthorized if the user does not have permission to view the campaign's users list, Ok otherwise.</returns>
    [HttpDelete("remove/{campaignGuid:guid}")]
    public async Task<IActionResult> RemoveUserToNotify(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignUsersList,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            await _notificationsService.RemoveUserToNotify(userId, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing user to notify");
            return StatusCode(500);
        }
    }
    
    /// <summary>
    /// Updates a user's existing notification settings for a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the related campaign.</param>
    /// <param name="notificationSettings">An instance of <see cref="NotificationSettings"/> with the settings the user
    /// wants.</param>
    /// <returns>Unauthorized if the user does not have permission to view the campaign's users list, Ok otherwise.</returns>
    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateUserToNotify(Guid campaignGuid, [FromBody] NotificationSettings notificationSettings)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                new Permission()
                {
                    PermissionTarget = PermissionTargets.CampaignUsersList,
                    PermissionType = PermissionTypes.View
                }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            await _notificationsService.UpdateUserToNotify(userId, campaignGuid, notificationSettings.ViaSms, notificationSettings.ViaEmail);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating user to notify");
            return StatusCode(500);
        }
    }
}