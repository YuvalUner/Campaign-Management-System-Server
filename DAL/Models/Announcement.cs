namespace DAL.Models;

public class Announcement
{
    public int? AnnouncementId { get; set; } 
    public string? AnnouncementContent { get; set; } 
    public int? PublisherId { get; set; } 
    public DateTime? PublishingDate { get; set; } 
    public int? CampaignId { get; set; } 
    public string? AnnouncementTitle { get; set; } 
    public Guid? AnnouncementGuid { get; set; } 
}