namespace DAL.Models;

public class AnnouncementsSearchParams
{
    public Guid? CampaignGuid { get; set; }
    public string? CampaignName { get; set; }
    public string? CampaignCity { get; set; }
    public DateTime? PublishingDate { get; set; }
    public string? AnnouncementTitle { get; set; }
    public string? PublisherFirstName { get; set; }
    public string? PublisherLastName { get; set; }
}