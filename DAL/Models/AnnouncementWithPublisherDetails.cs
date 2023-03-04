namespace DAL.Models;

public class AnnouncementWithPublisherDetails
{
    public DateTime? PublishingDate { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CampaignName { get; set; }
    public string? CampaignLogoUrl { get; set; }
    public Guid? CampaignGuid { get; set; }
    public Guid? AnnouncementGuid { get; set; }
    public string? AnnouncementTitle { get; set; }
    public string? AnnouncementContent { get; set; }
}