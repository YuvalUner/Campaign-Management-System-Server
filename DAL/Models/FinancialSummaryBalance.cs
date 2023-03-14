namespace DAL.Models;

/// <summary>
/// A model used to represent the balance of a financial type.<br/>
/// Contains the type's guid, name, balance, income total and expense total.<br/>
/// </summary>
public class FinancialSummaryBalance
{
    public Guid? TypeGuid { get; set; }
    public string? TypeName { get; set; }
    public Decimal? Balance { get; set; }
    public Decimal? IncomeTotal { get; set; }
    public Decimal? ExpenseTotal { get; set; }
}