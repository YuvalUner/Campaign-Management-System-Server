namespace DAL.Models;

public class FinancialSummaryEntry
{
    public Guid? TypeGuid { get; set; }
    public string? TypeName { get; set; }
    public bool IsExpense { get; set; }
    public Decimal? TotalAmount { get; set; }
}