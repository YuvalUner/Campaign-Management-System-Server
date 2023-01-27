using API.SessionExtensions;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : Controller
{
    private readonly ILogger<RolesController> _logger;
    private readonly IRolesService _rolesService;
    private readonly IUsersService _usersService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly ISmsMessageSendingService _smsMessageSendingService;
    private readonly ICampaignsService _campaignsService;
    
    public RolesController(ILogger<RolesController> logger, IRolesService rolesService, IUsersService usersService,
        IEmailSendingService emailSendingService, ISmsMessageSendingService smsMessageSendingService,
        ICampaignsService campaignsService)
    {
        _logger = logger;
        _rolesService = rolesService;
        _usersService = usersService;
        _emailSendingService = emailSendingService;
        _smsMessageSendingService = smsMessageSendingService;
        _campaignsService = campaignsService;
    }

    [HttpGet("get-roles/{campaignGuid:guid}")]
    public async Task<IActionResult> GetRoles(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRolesList,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized();
            }

            var roles = await _rolesService.GetRolesInCampaign(campaignGuid);
            return Ok(roles);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting roles in campaign");
            return StatusCode(500);
        }
    }
    
    [HttpPost("add-role/{campaignGuid:guid}")]
    public async Task<IActionResult> AddRole(Guid campaignGuid, [FromBody] Role role)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRolesList,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }
            
            if (role.RoleName == null)
            {
                return BadRequest("Role name cannot be null");
            }
            
            // Built-in roles cannot be added to campaigns.
            if (BuiltInRoleNames.IsBuiltInRole(role.RoleName))
            {
                return BadRequest("Can not add built-in roles to campaigns.");
            }

            var res = await _rolesService.AddRoleToCampaign(campaignGuid, role.RoleName, role.RoleDescription);
            if (res == -2)
            {
                return BadRequest("Too many roles in campaign");
            }
            if (res == -1)
            {
                return BadRequest("Role already exists");
            }
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding role to campaign");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("delete-role/{campaignGuid:guid}")]
    public async Task<IActionResult> DeleteRole(Guid campaignGuid, [FromBody] Role role)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRolesList,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }

            await _rolesService.DeleteRole(campaignGuid, role.RoleName);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting role from campaign");
            return StatusCode(500);
        }
    }
    
    [HttpPut("update-role/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateRole(Guid campaignGuid, [FromBody] Role role)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRolesList,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }

            await _rolesService.UpdateRole(campaignGuid, role.RoleName, role.RoleDescription);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating role in campaign");
            return StatusCode(500);
        }
    }
    
    [HttpPost("assign-role/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AssignRole(Guid campaignGuid, string userEmail,
        [FromBody] Role role, [FromQuery] NotificationSettings notificationSettings)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRoles,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }

            var res = await _rolesService.AssignUserToNormalRole(campaignGuid, userEmail, role.RoleName);
            switch (res)
            {
                case (int) ErrorCodes.RoleNotFound:
                    return BadRequest($"Error Num {res} - Role does not exist");
                case (int) ErrorCodes.UserNotFound:
                    return BadRequest($"Error Num {res} - User does not exist");
            }
            if (notificationSettings.ViaEmail || notificationSettings.ViaSms)
            {
                var campaign = await _campaignsService.GetCampaignNameByGuid(campaignGuid);
                if (notificationSettings.ViaEmail)
                {
                    _emailSendingService.SendRoleAssignedEmailAsync(role.RoleName, campaign, userEmail);
                }
                if (notificationSettings.ViaSms)
                {
                    var userToContact = await _usersService.GetUserContactInfoByEmail(userEmail);
                    if (userToContact != null && !string.IsNullOrEmpty(userToContact.PhoneNumber)){
                        _smsMessageSendingService.SendRoleAssignedSmsAsync(role.RoleName, campaign,
                            userToContact.PhoneNumber, CountryCodes.Israel);
                    }
                }
            }
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while assigning role to user");
            return StatusCode(500);
        }
    }
    
    [HttpPost("assign-admin-role/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> AssignAdminRole(Guid campaignGuid, string userEmail,
        [FromBody] Role role, [FromQuery] NotificationSettings notificationSettings)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRoles,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }
            
            var user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            // Check if the user is allowed to assign this role
            if (!await RoleUtils.CanAssignRole(_rolesService, HttpContext, role.RoleName, campaignGuid, user.UserId))
            {
                return Unauthorized();
            }

            var res = await _rolesService.AssignUserToAdministrativeRole(campaignGuid, userEmail, role.RoleName);
            switch (res)
            {
                 case (int) ErrorCodes.RoleNotFound:
                    return BadRequest($"Error Num {res} Role does not exist");
                case (int) ErrorCodes.UserNotFound:
                    return BadRequest($"User Num {res} - User does not exist");
            }
            if (notificationSettings.ViaEmail || notificationSettings.ViaSms)
            {
                var campaign = await _campaignsService.GetCampaignNameByGuid(campaignGuid);
                if (notificationSettings.ViaEmail)
                {
                    _emailSendingService.SendRoleAssignedEmailAsync(role.RoleName, campaign, userEmail);
                }
                if (notificationSettings.ViaSms)
                {
                    var userToContact = await _usersService.GetUserContactInfoByEmail(userEmail);
                    if (userToContact != null && !string.IsNullOrEmpty(userToContact.PhoneNumber)){
                        _smsMessageSendingService.SendRoleAssignedSmsAsync(role.RoleName, campaign,
                            userToContact.PhoneNumber, CountryCodes.Israel);
                    }
                }
            }
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while assigning admin role to user");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("remove-role/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> RemoveRole(Guid campaignGuid, string userEmail)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRoles,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }
            
            var user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            
            // Check if the user is allowed to remove this role
            if (!await RoleUtils.CanRemoveRole(_rolesService, HttpContext, campaignGuid, user.UserId))
            {
                return Unauthorized();
            }

            await _rolesService.RemoveUserFromRole(campaignGuid, userEmail);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing role from user");
            return StatusCode(500);
        }
    }
    
    [HttpDelete("remove-admin-role/{campaignGuid:guid}/{userEmail}")]
    public async Task<IActionResult> RemoveAdminRole(Guid campaignGuid, string userEmail)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CampaignRoles,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized();
            }
            
            var user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            
            // Check if the user is allowed to remove this role
            if (!await RoleUtils.CanRemoveRole(_rolesService, HttpContext, campaignGuid, user.UserId))
            {
                return Unauthorized();
            }

            await _rolesService.RemoveUserFromAdministrativeRole(campaignGuid, userEmail);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while removing administrative role from user");
            return StatusCode(500);
        }
    }
    
    [HttpGet("get-self-role/{campaignGuid:guid}")]
    public async Task<IActionResult> GetSelfRole(Guid campaignGuid)
    {
        // TODO: Call this using SignalR when the user's role changes.
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);
            
            var role = await _rolesService.GetRoleInCampaign(campaignGuid, userId);
            
            HttpContext.Session.Set(Constants.ActiveCampaignRole, role);

            return Ok(new
            {
                role?.RoleName,
                role?.RoleDescription
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting self role");
            return StatusCode(500);
        }
    }
}