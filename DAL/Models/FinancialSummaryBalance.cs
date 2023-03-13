namespace DAL.Models;

public class FinancialSummaryBalance
{
    public Guid? TypeGuid { get; set; }
    public string? TypeName { get; set; }
    public Decimal? Balance { get; set; }
    public Decimal? IncomeTotal { get; set; }
    public Decimal? ExpenseTotal { get; set; }
}