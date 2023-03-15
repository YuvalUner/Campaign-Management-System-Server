namespace DAL.Models;

/// <summary>
/// A model for the parameters used when searching for announcements.<br/>
/// All properties are nullable, so that the user can search for announcements with any combination of parameters.<br/>
/// </summary>
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