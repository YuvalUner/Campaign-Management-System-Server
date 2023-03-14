namespace DAL.Models;

/// <summary>
/// An extension of <see cref="FinancialDataEntry"/> that also includes the financial type's name and description,
/// as well as the details of the entry's creator.<br/>
/// </summary>
public class FinancialDataEntryWithTypeAndCreator: FinancialDataEntry
{
    public string? TypeName { get; set; }
    public string? TypeDescription { get; set; }
    public string? Email { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePicUrl { get; set; }
}