namespace DAL.Models;

/// <summary>
/// A model for a single entry of the financial_data table.<br/>
/// </summary>
public class FinancialDataEntry
{
    private readonly int _maxFinancialDataTitleLength = 50;
    private readonly int _maxFinancialDataDescriptionLength = 500;
    
    /// <summary>
    /// A collection of codes to use to determine success or failure of a validation operation of this model.<br/>
    /// </summary>
    public enum ValidationFailureCodes
    {
        Ok = 0,
        TitleTooLong = 1,
        DescriptionTooLong = 2,
        AmountTooLow = 3
    }
    
    /// <summary>
    /// Primary key of the financial_data table.<br/>
    /// Can be exposed to the client.<br/>
    /// </summary>
    public Guid? DataGuid { get; set; }
    public Guid? CampaignGuid { get; set; }
    
    /// <summary>
    /// Guid of the financial data type of the entry.<br/>
    /// Foreign key from the financial_types table.<br/> 
    /// </summary>
    public Guid? TypeGuid { get; set; }
    public int? CreatorUserId { get; set; }
    public bool IsExpense { get; set; } = false;
    public Decimal? Amount { get; set; }
    public string? DataTitle { get; set; }
    public string? DataDescription { get; set; }
    public DateTime? DateCreated { get; set; }

    /// <summary>
    /// A method that verifies that the values of the model are legal.<br/>
    /// </summary>
    /// <param name="isCreation">Whether the check is done for a create action or an update. Modifies the behavior
    /// of the method accordingly.</param>
    /// <returns><see cref="ValidationFailureCodes"/> Ok if the validation passes, a different code according to the
    /// first reason for failure otherwise.</returns>
    public ValidationFailureCodes VerifyLegalValues(bool isCreation = true)
    {
        if (DataTitle != null && DataTitle.Length > _maxFinancialDataTitleLength)
        {
            return ValidationFailureCodes.TitleTooLong;
        }
        
        if (DataDescription != null && DataDescription.Length > _maxFinancialDataDescriptionLength)
        {
            return ValidationFailureCodes.DescriptionTooLong;
        }
        
        if ((isCreation && Amount is null or <= 0) || (!isCreation && Amount is <= 0))
        {
            return ValidationFailureCodes.AmountTooLow;
        }
        
        return ValidationFailureCodes.Ok;
    }
}