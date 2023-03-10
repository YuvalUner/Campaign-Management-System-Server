namespace DAL.Models;

public class FinancialType
{
    public Guid? CampaignGuid { get; set; }
    public string? TypeName { get; set; }
    public string? TypeDescription { get; set; }
    public Guid? TypeGuid { get; set; }
}