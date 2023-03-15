namespace DAL.Models;

/// <summary>
/// An extension of <see cref="NotificationUponPublishSettings"/>, meant to be used when getting the notification settings
/// of a specific user.<br/>
/// </summary>
public class NotificationUponPublishSettingsForUser: NotificationUponPublishSettings
{
    public string? CampaignName { get; set; }
    public string? CampaignDescription { get; set; }
    public string? CampaignLogoUrl { get; set; }
}