namespace DAL.Models;

public class NotificationUponPublishSettings
{
    public int? UserId { get; set; }
    public Guid? CampaignGuid { get; set; }
    public bool? ViaSms { get; set; }
    public bool? ViaEmail { get; set; }
}