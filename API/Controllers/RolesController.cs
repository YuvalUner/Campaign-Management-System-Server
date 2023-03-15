using API.SessionExtensions;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIServices;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

/// <summary>
/// A controller for managing roles in campaigns.
/// Provides a web API and service policy for <see cref="IRolesService"/>.
/// </summary>
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

    /// <summary>
    /// Gets a list of all roles in a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>Unauthorized if user does not have permission to view roles in the campaign,
    /// Ok with a list of <see cref="Role"/> otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
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

    /// <summary>
    /// Adds a new role to a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="role">An instance of <see cref="Role"/>, containing all the required information.</param>
    /// <returns>Unauthorized if user does not have permission to edit roles in the campaign,
    /// BadRequest if the role name is empty or name of a built in role, the role already exists, or
    /// the campaign already has too many roles, Ok otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                return BadRequest(FormatErrorMessage(RoleNameRequired, CustomStatusCode.ValueCanNotBeNull));
            }

            // Built-in roles cannot be added to campaigns.
            if (BuiltInRoleNames.IsBuiltInRole(role.RoleName))
            {
                return BadRequest(FormatErrorMessage(NameMustNotBeBuiltIn, CustomStatusCode.NameCanNotBeBuiltIn));
            }

            var res = await _rolesService.AddRoleToCampaign(campaignGuid, role.RoleName, role.RoleDescription);
            switch (res)
            {
                case CustomStatusCode.TooManyEntries:
                    return BadRequest(FormatErrorMessage(ErrorMessages.TooManyRoles, res));
                case CustomStatusCode.RoleAlreadyExists:
                    return BadRequest(FormatErrorMessage(ErrorMessages.RoleAlreadyExists, res));
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding role to campaign");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Deletes an existing role from a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="role">An instance of <see cref="Role"/>, containing all the required information.</param>
    /// <returns>Unauthorized if user does not have permission to view roles in the campaign, Ok otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
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

    /// <summary>
    /// Updates an existing role's description in a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="role">An instance of <see cref="Role"/>, containing all the required information.</param>
    /// <returns>Unauthorized if user does not have permission to view roles in the campaign, Ok otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
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

    /// <summary>
    /// Adds a user to any non admin role.<br/>
    /// Also sends a notification to the user if the notification settings are set to true.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to assign to the role.</param>
    /// <param name="role">An instance of <see cref="Role"/>, containing all the required information.</param>
    /// <param name="notificationSettings">An instance of <see cref="NotificationSettings"/>, specifying
    /// whether to send a notification or not.</param>
    /// <returns>Unauthorized if user does not have permission to view roles in the campaign or the user
    /// can not assign to this role,
    /// BadRequest if the role or user were not found, Ok otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var res = await _rolesService.AssignUserToNormalRole(campaignGuid, userEmail, role.RoleName);
            switch (res)
            {
                case CustomStatusCode.RoleNotFound:
                    return BadRequest(FormatErrorMessage(RoleNotFound, res));
                case CustomStatusCode.UserNotFound:
                    return BadRequest(FormatErrorMessage(UserNotFound, res));
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
                    if (userToContact != null && !string.IsNullOrEmpty(userToContact.PhoneNumber))
                    {
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

    /// <summary>
    /// Assigns a user to an admin role and gives them full permissions.<br/>
    /// Also sends them a notification if requested.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to assign to the role.</param>
    /// <param name="role">An instance of <see cref="Role"/>, containing all the required information.</param>
    /// <param name="notificationSettings">An instance of <see cref="NotificationSettings"/>, specifying
    /// whether to send a notification or not.</param>
    /// <returns>Unauthorized if user does not have permission to view roles in the campaign or can not
    /// assign to this role,
    /// BadRequest if the role or user do not exist, or the user already has that role,
    /// Ok otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest(FormatErrorMessage(UserNotFound, CustomStatusCode.UserNotFound));
            }

            // Check if the user is allowed to assign this role
            if (!await RoleUtils.CanAssignRole(_rolesService, HttpContext, role.RoleName, campaignGuid, user.UserId))
            {
                return Unauthorized(FormatErrorMessage(PermissionError, CustomStatusCode.PermissionError));
            }

            var res = await _rolesService.AssignUserToAdministrativeRole(campaignGuid, userEmail, role.RoleName);
            switch (res)
            {
                case CustomStatusCode.RoleNotFound:
                    return BadRequest(FormatErrorMessage(ErrorMessages.RoleNotFound, res));
                case CustomStatusCode.UserNotFound:
                    return BadRequest(FormatErrorMessage(ErrorMessages.UserNotFound, res));
                case CustomStatusCode.DuplicateKey:
                    return BadRequest(FormatErrorMessage(ErrorMessages.RoleAlreadyAssigned, res));
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
                    if (userToContact != null && !string.IsNullOrEmpty(userToContact.PhoneNumber))
                    {
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

    /// <summary>
    /// Removes a user from a custom, non admin role.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to remove from the role.</param>
    /// <returns>Unauthorized if user does not have permission to view roles in the campaign or
    /// if they are not authorized to remove people from this role,
    /// BadRequest if the email does not belong to any user, Ok otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest(FormatErrorMessage(ErrorMessages.UserNotFound, CustomStatusCode.UserNotFound));
            }

            // Check if the user is allowed to remove this role
            if (!await RoleUtils.CanRemoveRole(_rolesService, HttpContext, campaignGuid, user.UserId))
            {
                return Unauthorized(FormatErrorMessage(PermissionError, CustomStatusCode.PermissionError));
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

    /// <summary>
    /// Removes a user from an administrative role, as well as removes all the permissions that were
    /// given to the user because of this role.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to remove from the role.</param>
    /// <returns>Unauthorized if user does not have permission to view roles in the campaign or if the
    /// requesting user can not remove someone from this role,
    /// BadRequest if the user does not exist, Ok otherwise.</returns>
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
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var user = await _usersService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return BadRequest(FormatErrorMessage(ErrorMessages.UserNotFound, CustomStatusCode.UserNotFound));
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

    /// <summary>
    /// Gets the user's own role within a campaign.<br/>
    /// Also sets the role in the session.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get the role for.</param>
    /// <returns>Ok with the <see cref="Role"/> retrieved if the user is part of the campaign, NotFound otherwise.</returns>
    [HttpGet("get-self-role/{campaignGuid:guid}")]
    public async Task<IActionResult> GetSelfRole(Guid campaignGuid)
    {
        // TODO: Call this using SignalR when the user's role changes.
        try
        {
            var userId = HttpContext.Session.GetInt32(Constants.UserId);

            var role = await _rolesService.GetRoleInCampaign(campaignGuid, userId);

            if (role != null)
            {
                HttpContext.Session.Set(Constants.ActiveCampaignRole, role);

                return Ok(new
                {
                    role?.RoleName,
                    role?.RoleDescription
                });
            }

            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting self role");
            return StatusCode(500);
        }
    }
}