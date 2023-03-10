namespace DAL.Models;

public class FinancialDataEntry
{
    public Guid? DataGuid { get; set; }
    public Guid? CampaignGuid { get; set; }
    public Guid? TypeGuid { get; set; }
    public int? CreatorUserId { get; set; }
    public bool IsExpense { get; set; } = false;
    public Decimal? Amount { get; set; }
    public string? DataTitle { get; set; }
    public string? DataDescription { get; set; }
    public DateTime? DateCreated { get; set; }
}