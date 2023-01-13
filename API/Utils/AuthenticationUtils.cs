using API.SessionExtensions;

namespace API.Utils;

public static class AuthenticationUtils
{
    /// <summary>
    /// Check if the campaign the user is trying to access is in their allowed campaigns list.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="campaignGuid"></param>
    /// <returns>true if yes, false otherwise</returns>
    public static bool IsUserAuthorizedForCampaign(HttpContext context, Guid? campaignGuid)
    {
        if (campaignGuid == null)
        {
            return false;
        }
        var allowedCampaigns = context.Session.Get<List<Guid?>?>(Constants.AllowedCampaigns);
        if (allowedCampaigns == null || !allowedCampaigns.Contains(campaignGuid))
        {
            return false;
        }

        return true;
    }
}