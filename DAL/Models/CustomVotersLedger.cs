namespace DAL.Models;

public class CustomVotersLedger
{
    public int? LedgerId { get; set; } 
    public int? CampaignId { get; set; } 
    public Guid? LedgerGuid { get; set; } 
    public string? LedgerName { get; set; } 
    public Guid? CampaignGuid { get; set; }
}
