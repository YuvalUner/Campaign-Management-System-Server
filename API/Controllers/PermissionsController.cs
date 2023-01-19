using API.Utils;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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

    [HttpPost("add/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AddPermission(Guid campaignGuid, string userEmail,
        [FromBody] Permission permission)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            // Check if user has permission to edit permissions
            var requiredPermission = new Permission()
            {
                PermissionType = PermissionTypes.Edit,
                PermissionTarget = PermissionTargets.Permissions
            };
            if (!PermissionUtils.HasPermission(HttpContext, requiredPermission))
            {
                return Unauthorized();
            }
            //  Check if user can edit this permission - can not edit permissions for other users if they do not have
            // that same permission themselves.
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

            await _permissionService.AddPermission(permission, user.UserId, campaignGuid);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding permission");
            return StatusCode(500);
        }
    }

    [HttpGet("getSelf/{campaignGuid:guid}")]
    public async Task<IActionResult> GetSelfPermissions(Guid campaignGuid)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            if (userId == null)
            {
                return Unauthorized();
            }

            var permissions = await _permissionService.GetPermissions(userId, campaignGuid);
            return Ok(permissions);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting permissions");
            return StatusCode(500);
        }
    }

    [HttpGet("get/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> GetPermissions(Guid campaignGuid, string userEmail)
    {
        try
        {
            if (!CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(HttpContext, campaignGuid)
                || !CampaignAuthorizationUtils.DoesActiveCampaignMatch(HttpContext, campaignGuid))
            {
                return Unauthorized();
            }

            var requiredPermission = new Permission()
            {
                PermissionTarget = PermissionTargets.Permissions,
                PermissionType = PermissionTypes.View
            };
            if (!PermissionUtils.HasPermission(HttpContext, requiredPermission))
            {
                return Unauthorized();
            }

            User? user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound();
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