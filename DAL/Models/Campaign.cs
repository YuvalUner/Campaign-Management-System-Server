namespace DAL.Models;

public class Campaign
{
    public int CampaignId { get; set; }
    public string? CampaignName { get; set; }
    public string? CampaignDescription { get; set; }
    public User? CampaignCreator { get; set; }
    public int? CampaignCreatorUserId { get; set; }
    public DateTime? CampaignCreationDate { get; set; }
    public Guid? CampaignGuid { get; set; }
    public bool? CampaignIsActive { get; set; }
    public IEnumerable<User?>? CampaignUsers { get; set; }
    public Guid? CampaignInviteGuid { get; set; }
}