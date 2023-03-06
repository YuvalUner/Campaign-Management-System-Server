namespace DAL.Models;

public class EventsSearchParams
{
    public Guid? CampaignGuid { get; set; }
    public string? CampaignName { get; set; }
    public string? CampaignCity { get; set; }
    public DateTime? PublishingDate { get; set; }
    public string? EventName { get; set; }
    public string? PublisherFirstName { get; set; }
    public string? PublisherLastName { get; set; }
    public string? EventLocation { get; set; }
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
}