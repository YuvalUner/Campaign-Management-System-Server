namespace DAL.Models;

public class NotificationUponPublishSettingsForCampaign
{
    public bool? ViaSms { get; set; }
    public bool? ViaEmail { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}