namespace DAL.Models;
using DbAccess;

/// <summary>
/// A model meant to represent a single returning row from the <see cref="StoredProcedureNames.GetFinancialSummaryForCampaign"/>
/// stored procedure.<br/>
/// Each row represent a single type of financial entry, and the total amount of either expense or income for that
/// entry.<br/>
/// </summary>
public class FinancialSummaryEntry
{
    public Guid? TypeGuid { get; set; }
    public string? TypeName { get; set; }
    public bool IsExpense { get; set; }
    public Decimal? TotalAmount { get; set; }
}