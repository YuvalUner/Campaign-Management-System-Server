namespace DAL.Models;

public class Campaign
{
    public int? CampaignId { get; set; } 
    public string? CampaignName { get; set; } 
    public Guid? CampaignGuid { get; set; } 
    public int? CampaignCreatorUserId { get; set; } 
    public string? CampaignDescription { get; set; } 
    public DateTime? CampaignCreationDate { get; set; } 
    public bool? CampaignIsActive { get; set; } 
    public Guid? CampaignInviteGuid { get; set; } 
    public bool? IsMunicipal { get; set; } 
    public bool? IsSubCampaign { get; set; } 
    public string? CampaignLogoUrl { get; set; } 
}