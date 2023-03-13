using DAL.Models;

namespace API.Utils;

/// <summary>
/// A class made for convenience, to save on boilerplate code when verifying if a user should
/// be allowed to perform the action they are currently trying to perform.<br/>
/// A combination of the checks in <see cref="CampaignAuthorizationUtils"/> and <see cref="PermissionUtils"/>,
/// checking if the user is a member of the campaign, if the campaign is the active campaign, and if the user has the
/// required permission.<br/>
/// Only contains the <see cref="IsUserAuthorizedForCampaignAndHasPermission"/> method.<br/>
/// </summary>
public static class CombinedPermissionCampaignUtils
{
    /// <summary>
    /// Checks if the user is a member of the campaign, if the campaign is the active campaign, and if the user has the
    /// required permission.<br/>
    /// </summary>
    /// <param name="context">The HttpContext of the controller requesting the check.</param>
    /// <param name="campaignGuid">The Guid of the campaign to check for.</param>
    /// <param name="permission">The permission to check existence of.</param>
    /// <returns></returns>
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