﻿namespace DAL.Models;

public class PublishedEventWithPublisher
{
    public Guid EventGuid { get; set; }
    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }
    public string? EventLocation { get; set; }
    public int? MaxAttendees { get; set; }
    public int? NumAttending { get; set; }
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
}