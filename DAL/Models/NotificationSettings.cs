namespace DAL.Models;

public class NotificationSettings
{
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool ViaEmail { get; set; }
    public bool ViaSms { get; set; }
}