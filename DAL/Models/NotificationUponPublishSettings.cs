namespace DAL.Models;

/// <summary>
/// A model for the settings needed when a user wants to be notified when a campaign publishes something.<br/>
/// Can be used for both signing up for the notifications, as well as sending the notifications.<br/>
/// </summary>
public class NotificationUponPublishSettings
{
    public int? UserId { get; set; }
    public Guid? CampaignGuid { get; set; }
    
    /// <summary>
    /// Set to true if the user wants to be notified via SMS.<br/>
    /// </summary>
    public bool? ViaSms { get; set; }
    
    /// <summary>
    /// Set to true if the user wants to be notified via email.<br/>
    /// </summary>
    public bool? ViaEmail { get; set; }
}