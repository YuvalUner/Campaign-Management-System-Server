namespace DAL.Models;

public class SmsSendingParams
{
    public int? SenderId { get; set; }
    public Guid? CampaignGuid { get; set; }
    public List<string?> PhoneNumbers { get; set; }
    public string? MessageContents { get; set; }
}