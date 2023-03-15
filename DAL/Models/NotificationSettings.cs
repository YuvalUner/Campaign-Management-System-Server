namespace DAL.Models;

/// <summary>
/// A model for the settings of user for receiving certain notifications.<br/>
/// Contains both the fields needed to sign up for the notifications, as well as the fields needed to send the notifications.<br/>
/// </summary>
public class NotificationSettings
{
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool ViaEmail { get; set; }
    public bool ViaSms { get; set; }
}