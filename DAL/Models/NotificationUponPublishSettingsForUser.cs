namespace DAL.Models;

public class NotificationUponPublishSettingsForUser
{
    public bool? ViaSms { get; set; }
    public bool? ViaEmail { get; set; }
    public string? CampaignName { get; set; }
    public Guid? CampaignGuid { get; set; }
    public string? CampaignDescription { get; set; }
    public string? CampaignLogoUrl { get; set; }
}