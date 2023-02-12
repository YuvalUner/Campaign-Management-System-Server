namespace DAL.Models;

public class SmsSendingParams
{
    public int? UserId { get; set; }
    public Guid? CampaignGuid { get; set; }
    public IEnumerable<string?> PhoneNumbers { get; set; }
    public string? MessageContents { get; set; }
}