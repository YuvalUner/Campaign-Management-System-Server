using API.SessionExtensions;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for handling permissions.
/// Provides a web API and service policy for <see cref="IPermissionsService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : Controller
{
    private readonly IPermissionsService _permissionService;
    private readonly ILogger<PermissionsController> _logger;
    private readonly IUsersService _usersService;
    private readonly IRolesService _rolesService;

    public PermissionsController(IPermissionsService permissionService, ILogger<PermissionsController> logger,
        IUsersService usersService, IRolesService rolesService)
    {
        _permissionService = permissionService;
        _logger = logger;
        _usersService = usersService;
        _rolesService = rolesService;
    }

    /// <summary>
    /// Adds a new permission to a user within a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to add the permission to.</param>
    /// <param name="permission">A <see cref="Permission"/>, the permission to add.</param>
    /// <returns>Unauthorized if the requesting user can not edit permissions in the campaign or can not add this permission to
    /// this user, NotFound if the user to add the permission to or the permission itself do not exist,
    /// BadRequest if the user already has this permission, Ok otherwise.</returns>
    [HttpPost("add/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AddPermission(Guid campaignGuid, string userEmail,
        [FromBody] Permission permission)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Permissions,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError, CustomStatusCode.AuthorizationError));
            }
            
            User? user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound(FormatErrorMessage(UserNotFound, CustomStatusCode.UserNotFound));
            }
            
            // Check if the user is permitted to delete this permission
            if (!await PermissionUtils.CanEditPermission(HttpContext, permission, user, _rolesService))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError, CustomStatusCode.AuthorizationError));
            }

            var res = await _permissionService.AddPermission(permission, user.UserId, campaignGuid);
            return res switch
            {
                CustomStatusCode.PermissionDoesNotExist => NotFound(FormatErrorMessage(PermissionNotFound, res)),
                CustomStatusCode.UserAlreadyHasPermission => BadRequest(FormatErrorMessage(UserAlreadyHasPermission, res)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding permission");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets the permissions of the currently logged in user within a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>Unauthorized if user does not have access to this campaign,
    /// Ok with a list of <see cref="Permission"/> otherwise.</returns>
    [HttpGet("getSelf/{campaignGuid:guid}")]
    public async Task<IActionResult> GetSelfPermissions(Guid campaignGuid)
    {
        // TODO: Use SignalR to call this method when a permission is changed.
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized(FormatErrorMessage(AuthorizationError, CustomStatusCode.AuthorizationError));
            }
            
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            // This should never happen, as the user should be logged in to access this method.
            // I have no idea why my past self did this, but my current self is too scared to change it.
            if (userId == null)
            {
                return BadRequest(FormatErrorMessage(UserNotFound, CustomStatusCode.ValueNotFound));
            }
            

            var permissions = await _permissionService.GetPermissions(userId, campaignGuid);
            // Update user permissions in the session, as this method may be called after a permission change.
            HttpContext.Session.Set(Constants.Permissions, permissions);
            return Ok(permissions);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting permissions");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets the permissions of a specific user within a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to get the permissions of.</param>
    /// <returns>Unauthorized if the requesting user can not view permissions in the campaign,
    /// NotFound if there is no user with that email, Ok with a list of <see cref="Permission"/> otherwise.</returns>
    [HttpGet("get/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> GetPermissions(Guid campaignGuid, string userEmail)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.Permissions,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.AuthorizationError));
            }

            User? user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound(FormatErrorMessage(UserNotFound, CustomStatusCode.ValueNotFound));
            }

            var permissions = await _permissionService.GetPermissions(user.UserId, campaignGuid);
            return Ok(permissions);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting permissions");
            return StatusCode(500);
        }
    }
    
    /// <summary>
    /// Removes a permission from a user within a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to remove the permission from.</param>
    /// <param name="permission">A <see cref="Permission"/>, the permission to add.</param>
    /// <returns>Unauthorized if the requesting user can not edit permissions in the campaign or can not remove
    /// this permission to from this user, Ok otherwise.</returns>
    [HttpDelete("delete/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> DeletePermission(Guid campaignGuid, string userEmail,
        [FromBody] Permission permission)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }
            
            User? user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is permitted to delete this permission
            if (!await PermissionUtils.CanEditPermission(HttpContext, permission, user, _rolesService))
            {
                return Unauthorized();
            }

            await _permissionService.RemovePermission(permission, user.UserId, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing permission");
            return StatusCode(500);
        }
    }
}