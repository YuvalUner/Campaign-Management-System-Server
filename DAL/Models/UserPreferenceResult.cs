namespace DAL.Models;

using DbAccess;

/// <summary>
/// A model for a single row of the <see cref="StoredProcedureNames.GetUserPreferences"/> stored procedure.<br/>
/// Contains whether the user a preference or wishes to avoid a campaign, and the campaign's name, Guid and logo.<br/>
/// </summary>
public class UserPreferenceResult
{
    /// <summary>
    /// True if the user has a preference for the campaign, false if the user wishes to avoid the campaign.
    /// </summary>
    public bool? IsPreferred { get; set; }

    public Guid? CampaignGuid { get; set; }
    public string? CampaignName { get; set; }
    public string? CampaignLogoUrl { get; set; }
}