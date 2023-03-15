namespace DAL.Models;

/// <summary>
/// A model for the custom_events table.<br/>
/// </summary>
public class CustomEvent
{
    /// <summary>
    /// Id - the primary key, auto incremented.<br/>
    /// Not to be exposed to the user.<br/>
    /// </summary>
    public int? EventId { get; set; }

    /// <summary>
    /// Guid - a unique identifier for the event, used to differentiate it.<br/>
    /// Can and should be exposed to the user.<br/>
    /// </summary>
    public Guid? EventGuid { get; set; }

    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public string? EventLocation { get; set; }
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
    public int? CampaignId { get; set; }
    public int? MaxAttendees { get; set; }
    public int? NumAttending { get; set; }
    
    /// <summary>
    /// User id of the user who created the event.<br/>
    /// </summary>
    public int? EventCreatorId { get; set; }

    public Guid? CampaignGuid { get; set; }

    public bool? IsOpenJoin { get; set; }

    public int? EventOf { get; set; }
}