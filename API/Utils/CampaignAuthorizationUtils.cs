using API.SessionExtensions;

namespace API.Utils;

public static class CampaignAuthorizationUtils
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

    public static void AddAuthorizationForCampaign(HttpContext context, Guid? campaignGuid)
    {
        if (campaignGuid == null)
        {
            return;
        }
        var allowedCampaigns = context.Session.Get<List<Guid?>?>(Constants.AllowedCampaigns);
        if (allowedCampaigns == null)
        {
            allowedCampaigns = new List<Guid?>();
        }
        if (!allowedCampaigns.Contains(campaignGuid))
        {
            allowedCampaigns.Add(campaignGuid);
        }
        context.Session.Set(Constants.AllowedCampaigns, allowedCampaigns);
    }

    public static void EnterCampaign(HttpContext context, Guid? campaignGuid)
    {
        context.Session.Set<Guid?>(Constants.ActiveCampaign, campaignGuid);
    }
    
    public static bool DoesActiveCampaignMatch(HttpContext context, Guid? campaignGuid)
    {
        var activeCampaign = context.Session.Get<Guid?>(Constants.ActiveCampaign);
        return activeCampaign == campaignGuid;
    }
}