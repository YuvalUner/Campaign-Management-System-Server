using API.SessionExtensions;

namespace API.Utils;

/// <summary>
/// A collection of methods used for checking if the user is authorized to access a campaign, if the campaign is the active
/// campaign of the user, and for adding and setting campaigns and the active campaign.<br/>
/// All checks and additions are done to the session.<br/>
/// </summary>
public static class CampaignAuthorizationUtils
{
    /// <summary>
    /// Check if the campaign the user is trying to access is in their allowed campaigns list.
    /// </summary>
    /// <param name="context">The HttpContext object of the controller requesting the check.</param>
    /// <param name="campaignGuid">The Guid of the campaign to check for.</param>
    /// <returns>true if the campaign is in the allowed list, false otherwise.</returns>
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

    /// <summary>
    /// Adds the campaign to the allowed campaigns list.<br/>
    /// </summary>
    /// <param name="context">The HttpContext object of the controller.</param>
    /// <param name="campaignGuid">The Guid of the campaign to add to the session. Future sessions should already
    /// have the Guid in them, as it will be retrieved from the database instead.</param>
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

    /// <summary>
    /// Sets the active campaign in the session.<br/>
    /// </summary>
    /// <param name="context">The HttpContext object of the controller.</param>
    /// <param name="campaignGuid">The Guid of the campaign to set as the current active campaign.</param>
    public static void EnterCampaign(HttpContext context, Guid? campaignGuid)
    {
        context.Session.Set<Guid?>(Constants.ActiveCampaign, campaignGuid);
    }

    /// <summary>
    /// Checks if the active campaign in the session matches the campaignGuid.<br/>
    /// </summary>
    /// <param name="context">The HttpContext object of the controller.</param>
    /// <param name="campaignGuid">The Guid of the campaign that the controller received.</param>
    /// <returns>True if the Guids match, false otherwise.</returns>
    public static bool DoesActiveCampaignMatch(HttpContext context, Guid? campaignGuid)
    {
        var activeCampaign = context.Session.Get<Guid?>(Constants.ActiveCampaign);
        return activeCampaign == campaignGuid;
    }
}