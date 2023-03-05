namespace DAL.Models;

public class UserPreferenceResult
{
    public bool? IsPreferred { get; set; }
    public Guid? CampaignGuid { get; set; }
    public string? CampaignName { get; set; }
    public string? CampaignLogoUrl { get; set; }
}