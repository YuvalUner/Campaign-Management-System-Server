using API.SessionExtensions;
using DAL.Models;
using DAL.Services.Interfaces;

namespace API.Utils;

/// <summary>
/// A collection of utility methods for permissions.<br/>
/// Checks if the user has a specific permission, or if they can edit a specific permission ,and can also
/// set the user's permissions in the session.<br/>
/// </summary>
public static class PermissionUtils
{
    /// <summary>
    /// Sets the user's permissions in the session.<br/>
    /// </summary>
    /// <param name="permissionsService">An implementation of <see cref="IPermissionsService"/></param>
    /// <param name="context">The HttpContext object of the controller.</param>
    public static async Task SetPermissions(IPermissionsService permissionsService, HttpContext context)
    {
        var userId = context.Session.GetInt32(Constants.UserId);
        var campaignGuid = context.Session.Get<Guid?>(Constants.ActiveCampaign);

        var permissions = await permissionsService.GetPermissions(userId, campaignGuid);

        context.Session.Set(Constants.Permissions, permissions);
    }
    
    /// <summary>
    /// Checks if the user has a specific permission.<br/>
    /// </summary>
    /// <param name="context">The HttpContext object of the controller.</param>
    /// <param name="permission">A <see cref="Permission"/> object with the permission target and type to check.</param>
    /// <returns>True if the permission list in the session contains the permission, false otherwise.</returns>
    public static bool HasPermission(HttpContext context, Permission permission)
    {
        var permissions = context.Session.Get<List<Permission?>>(Constants.Permissions);
        return permissions.Contains(permission);
    }

    /// <summary>
    /// Checks if the user can edit a specific permission (add or remove it from a user)
    /// , according to the permission policy that was decided upon.<br/>
    /// </summary>
    /// <param name="context">The HttpContext object of the controller.</param>
    /// <param name="permission">The permission that the user is requesting to edit.</param>
    /// <param name="otherUser">The <see cref="User"/> object representing the other user. Must have the UserId property set.</param>
    /// <param name="rolesService">An implementation of <see cref="IRolesService"/></param>
    /// <returns>True if the user can perform the edit, false otherwise.</returns>
    public static async Task<bool> CanEditPermission(HttpContext context, Permission permission,
        User otherUser, IRolesService rolesService)
    {
        // First check - does the user have the permission to edit permissions?
        var editPermissionsPermission = new Permission()
        {
            PermissionType = PermissionTypes.Edit,
            PermissionTarget = PermissionTargets.Permissions
        };
        if (!HasPermission(context, editPermissionsPermission))
        {
            return false;
        }
        // Second check - does the user have the permission to edit the permission they're trying to edit?
        // This is determined by them also having at-least a view permission for the same target.
        // This is done to prevent users from giving permissions to others that they do not have themselves.
        var requiredPermission = new Permission()
        {
            PermissionType = PermissionTypes.View,
            PermissionTarget = permission.PermissionTarget
        };
        if (!HasPermission(context, requiredPermission))
        {
            return false;
        }

        // If the user is attempting to modify their own permissions and have passed the above 2 tests, they can do so.
        // This is an unlikely scenario, but it's possible.
        var userId = context.Session.GetInt32(Constants.UserId);
        if (userId == otherUser.UserId)
        {
            return true;
        }
        
        // Third check - make sure the user is not trying to modify the permissions of someone with a higher or equal role level.
        // This is done to make sure that users cannot modify the permissions of admins.
        // Role levels are defined in the Roles table.
        var userRole = context.Session.Get<Role?>(Constants.ActiveCampaignRole);
        var campaignGuid = context.Session.Get<Guid?>(Constants.ActiveCampaign);
        if (userRole == null || campaignGuid == null)
        {
            return false;
        }
        
        var otherUserRole = await rolesService.GetRoleInCampaign(campaignGuid, otherUser.UserId);
        // If the other user has a higher or equal role level, then the user cannot modify their permissions.
        // However, roles with level 0 - common, non admin roles, can be modified by anyone that passes the above tests.
        if (otherUserRole == null 
            || (userRole.RoleLevel == 0 && otherUserRole.RoleLevel > userRole.RoleLevel)
            || (userRole.RoleLevel != 0 && otherUserRole.RoleLevel >= userRole.RoleLevel))
        {
            return false;
        }
        return true;
    }
}