namespace DAL.Models;

/// <summary>
/// An extension of <see cref="CustomEvent"/>, meant for events that have been published.<br/>
/// Also includes the publisher's details and the campaign's info.<br/>
/// </summary>
public class PublishedEventWithPublisher: CustomEvent
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
}