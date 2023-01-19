using API.SessionExtensions;
using DAL.Models;
using DAL.Services.Interfaces;

namespace API.Utils;

public static class PermissionUtils
{
    public static async Task SetPermissions(IPermissionsService permissionsService, HttpContext context)
    {
        var userId = context.Session.GetInt32(Constants.UserId);
        var campaignGuid = context.Session.Get<Guid?>(Constants.ActiveCampaign);

        var permissions = await permissionsService.GetPermissions(userId, campaignGuid);

        context.Session.Set(Constants.Permissions, permissions);
    }
    
    public static bool HasPermission(HttpContext context, Permission permission)
    {
        var permissions = context.Session.Get<List<Permission?>>(Constants.Permissions);
        return permissions.Contains(permission);
    }

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
        if (otherUserRole == null || (userRole.RoleLevel != 0 && otherUserRole.RoleLevel >= userRole.RoleLevel))
        {
            return false;
        }
        return true;
    }
}