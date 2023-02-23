namespace DAL.Models;

public class GetUserEventsResult
{
    public Guid EventGuid { get; set; }
    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public string? EventLocation { get; set; }
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
    public int? MaxAttendees { get; set; }
    public Guid? CampaignGuid { get; set; }
    public string? CampaignName { get; set; }
    public string? CampaignLogoUrl { get; set; }
    public bool Participating { get; set; }
    
    public bool IsOpenJoin { get; set; } = false;
}