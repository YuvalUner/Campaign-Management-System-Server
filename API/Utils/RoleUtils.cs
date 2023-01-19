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
}