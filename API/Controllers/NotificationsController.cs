using System.Reflection.Metadata;
using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : Controller
{
    private readonly INotificationsService _notificationsService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationsService notificationsService, ILogger<NotificationsController> logger)
    {
        _notificationsService = notificationsService;
        _logger = logger;
    }

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
                return Unauthorized();
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
                return Unauthorized();
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
                return Unauthorized();
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            await _notificationsService.ModifyUserToNotify(userId, campaignGuid, notificationSettings.ViaSms, notificationSettings.ViaEmail);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating user to notify");
            return StatusCode(500);
        }
    }
}