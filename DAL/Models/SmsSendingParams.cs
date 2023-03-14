namespace DAL.Models;

/// <summary>
/// A model for the parameters needed when sending an SMS.<br/>
/// </summary>
public class SmsSendingParams
{
    public int? SenderId { get; set; }
    public Guid? CampaignGuid { get; set; }
    public List<string?>? PhoneNumbers { get; set; }
    public string? MessageContents { get; set; }
}