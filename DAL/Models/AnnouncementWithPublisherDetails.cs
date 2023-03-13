namespace DAL.Models;

/// <summary>
/// Like <see cref="Announcement"/>, but also includes the publisher's details, as well as the details of the
/// campaign the announcement is related to.<br/>
/// </summary>
public class AnnouncementWithPublisherDetails
{
    public DateTime? PublishingDate { get; set; }
    
    /// <summary>
    /// The first name of the publisher (provided by them), in Hebrew.<br/>
    /// </summary>
    public string? FirstNameHeb { get; set; }
    
    /// <summary>
    /// The last name of the publisher (provided by them), in Hebrew.<br/>
    /// </summary>
    public string? LastNameHeb { get; set; }
    
    /// <summary>
    /// The display name of the publisher, as provided by Google when they signed up, in English.<br/>
    /// </summary>
    public string? DisplayNameEng { get; set; }
    
    /// <summary>
    /// Url to the profile picture of the publisher, provided by Google on sign up.<br/>
    /// </summary>
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