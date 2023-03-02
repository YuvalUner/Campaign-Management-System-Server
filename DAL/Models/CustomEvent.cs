namespace DAL.Models;

public class CustomEvent
{
    public int? EventId { get; set; } 
    public Guid? EventGuid { get; set; } 
    public string? EventName { get; set; } 
    public string? EventDescription { get; set; } 
    public string? EventLocation { get; set; } 
    public DateTime? EventStartTime { get; set; } 
    public DateTime? EventEndTime { get; set; } 
    public int? CampaignId { get; set; } 
    public int? MaxAttendees { get; set; } 
    public int? NumAttending { get; set; } 
    public int? EventCreatorId { get; set; } 
    
    public Guid? CampaignGuid { get; set; }

    public bool? IsOpenJoin { get; set; }
    
    public int? EventOf { get; set; }
}