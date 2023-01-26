using DAL.Models;

namespace API.Utils;

public static class CombinedPermissionCampaignUtils
{
    public static bool IsUserAuthorizedForCampaignAndHasPermission(HttpContext context, Guid campaignGuid,
        Permission permission)
    {
        if (CampaignAuthorizationUtils.IsUserAuthorizedForCampaign(context, campaignGuid)
            && CampaignAuthorizationUtils.DoesActiveCampaignMatch(context, campaignGuid)
            && PermissionUtils.HasPermission(context, permission))
        {
            return true;
        }

        return false;
    }
}