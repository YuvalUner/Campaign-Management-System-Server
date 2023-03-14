namespace DAL.Models;

/// <summary>
/// A model for the campaigns table<br/>
/// On campaign creation, CampaignName, IsMunicipal, IsSubCampaign, CityName are all required.<br/>
/// Others are either optional or will be filled automatically.<br/>
/// </summary>
public class Campaign
{
    /// <summary>
    /// The id of the campaign.<br/>
    /// An auto-incremented value that acts as the primary key.<br/>
    /// Never exposed to the user.<br/>
    /// </summary>
    public int? CampaignId { get; set; }

    public string? CampaignName { get; set; }

    /// <summary>
    /// The guid of the campaign.<br/>
    /// Auto generated when the campaign is created.<br/>
    /// This is the unique identifier used for each campaign, and should be exposed to the user.<br/>
    /// </summary>
    public Guid? CampaignGuid { get; set; }

    public int? CampaignCreatorUserId { get; set; }
    public string? CampaignDescription { get; set; }
    public DateTime? CampaignCreationDate { get; set; }
    public bool? CampaignIsActive { get; set; }
    public Guid? CampaignInviteGuid { get; set; }
    public bool? IsMunicipal { get; set; }

    /// <summary>
    /// Set to true if the campaign is a sub-campaign of another, larger in scope campaign.<br/>
    /// For example, a campaign for a specific city being part of a larger campaign for the entire country.<br/>
    /// </summary>
    public bool? IsSubCampaign { get; set; }

    public string? CampaignLogoUrl { get; set; }
    public int? CityId { get; set; }
    public string? CityName { get; set; }
}