using API.SessionExtensions;
using DAL.Models;
using DAL.Services.Interfaces;

namespace API.Utils;

public static class RoleUtils
{
    public static async Task SetRole(IRolesService rolesService, HttpContext context)
    {
        var userId = context.Session.GetInt32(Constants.UserId);
        var campaignGuid = context.Session.Get<Guid?>(Constants.ActiveCampaign);
        
        if (userId.HasValue && campaignGuid.HasValue)
        {
            var role = await rolesService.GetRoleInCampaign(campaignGuid.Value, userId);
            context.Session.Set(Constants.ActiveCampaignRole, role);
        }
    }

    /// <summary>
    /// Checks if a user can assign a role to another user.
    /// They are allowed to do so if their role level is higher or if they are the campaign owner, as well as
    /// the user they are assigning the role having a lower role level than them.
    /// </summary>
    /// <param name="rolesService"></param>
    /// <param name="context"></param>
    /// <param name="roleName"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static async Task<bool> CanAssignRole(IRolesService rolesService, HttpContext context,
        string? roleName, Guid? campaignGuid, int? userId)
    {

        var activeCampaignRole = context.Session.Get<Role?>(Constants.ActiveCampaignRole);
        if (activeCampaignRole == null)
        {
            return false;
        }
        
        var userRole = await rolesService.GetRoleInCampaign(campaignGuid, userId);
        if (userRole == null)
        {
            return false;
        }
        
        // If the other user's role level is higher than the user's, they can't assign a role to them.
        if (userRole.RoleLevel > activeCampaignRole.RoleLevel)
        {
            return false;
        }
        
        
        // If you're the owner, you can assign any role, including the owner role.
        if (activeCampaignRole.RoleName == BuiltInRoleNames.Owner)
        {
            return true;
        }
        
        var roleToAssign = await rolesService.GetRole(roleName, campaignGuid);
        if (roleToAssign == null)
        {
            return false;
        }
        
        // If the user's role level is higher than the role they are assigning, they can assign it.
        // Alternatively, both users have a role level of 0, AKA not admin level, therefore it is 
        // purely a matter of permissions.
        return activeCampaignRole.RoleLevel > userRole.RoleLevel
               || (activeCampaignRole.RoleLevel == 0 && activeCampaignRole.RoleLevel == userRole.RoleLevel);

    }
    
    /// <summary>
    /// Checks whether a user can remove a role from another user.
    /// A role can only be removed by someone with a higher role level.
    /// Unlike the previous methods, this one does not check if the user is the owner.
    /// That is because even campaign owners can't remove the owner role from other users.
    /// This is a security measure to prevent internal sabotage.
    /// </summary>
    /// <param name="rolesService"></param>
    /// <param name="context"></param>
    /// <param name="campaignGuid"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static async Task<bool> CanRemoveRole(IRolesService rolesService, HttpContext context,
        Guid? campaignGuid, int? userId)
    {

        var activeCampaignRole = context.Session.Get<Role?>(Constants.ActiveCampaignRole);
        if (activeCampaignRole == null)
        {
            return false;
        }
        

        var userRole = await rolesService.GetRoleInCampaign(campaignGuid, userId);
        if (userRole == null)
        {
            return false;
        }
        
        return activeCampaignRole.RoleLevel > userRole.RoleLevel
            || (activeCampaignRole.RoleLevel == 0 && activeCampaignRole.RoleLevel == userRole.RoleLevel);
        
    }
}