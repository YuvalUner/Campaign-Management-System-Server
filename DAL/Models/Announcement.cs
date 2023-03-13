namespace DAL.Models;

/// <summary>
/// A model for the public_board_announcements table.
/// </summary>
public class Announcement
{
    /// <summary>
    /// The Id of the announcement.<br/>
    /// It is an auto-incremented value that acts as the primary key.<br/>
    /// Should not be exposed to the user.<br/>
    /// </summary>
    public int? AnnouncementId { get; set; } 
    
    /// <summary>
    /// The content of the announcement.<br/>
    /// Generally, any get method should return this value as part of it.<br/>
    /// </summary>
    public string? AnnouncementContent { get; set; } 
    
    /// <summary>
    /// The user id of the user who created the announcement.<br/>
    /// Generally, only needed when creating the announcement.<br/>
    /// For getting the publisher as well, see <see cref="AnnouncementWithPublisherDetails"/>.<br/>
    /// </summary>
    public int? PublisherId { get; set; } 
    
    /// <summary>
    /// The date when the announcement was published.<br/>
    /// </summary>
    public DateTime? PublishingDate { get; set; } 
    
    /// <summary>
    /// The campaign id of the campaign that the announcement is related to.<br/>
    /// Generally, only needed when creating the announcement.<br/>
    /// Should not be exposed to the user.<br/>
    /// </summary>
    public int? CampaignId { get; set; } 
    
    public string? AnnouncementTitle { get; set; } 
    
    /// <summary>
    /// The guid of the announcement.<br/>
    /// A unique identifier for the announcement, used to differentiate it.<br/>
    /// This can be exposed to the user.<br/>
    /// </summary>
    public Guid? AnnouncementGuid { get; set; } 
}