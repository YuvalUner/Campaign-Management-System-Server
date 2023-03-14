namespace DAL.Models;

/// <summary>
/// An extension of <see cref="NotificationUponPublishSettings"/>, meant to be used when getting the notification settings
/// for a specific campaign.<br/>
/// This is used when sending the notifications to all the users that signed up for the notifications.<br/>
/// </summary>
public class NotificationUponPublishSettingsForCampaign: NotificationUponPublishSettings
{
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}