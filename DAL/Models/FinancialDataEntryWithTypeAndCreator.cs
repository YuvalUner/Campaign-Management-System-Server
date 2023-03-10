namespace DAL.Models;

public class FinancialDataEntryWithTypeAndCreator
{
    public Guid? DataGuid { get; set; }
    public Guid? TypeGuid { get; set; }
    public string? TypeName { get; set; }
    public string? TypeDescription { get; set; }
    public bool IsExpense { get; set; }
    public Decimal? Amount { get; set; }
    public string? DataTitle { get; set; }
    public string? DataDescription { get; set; }
    public DateTime? DateCreated { get; set; }
    public string? Email { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePicUrl { get; set; }
}