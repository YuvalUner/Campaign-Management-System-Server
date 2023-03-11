namespace DAL.Models;

public class FinancialDataEntry
{
    private readonly int _maxFinancialDataTitleLength = 50;
    private readonly int _maxFinancialDataDescriptionLength = 500;
    
    public enum ValidationFailureCodes
    {
        Ok = 0,
        TitleTooLong = 1,
        DescriptionTooLong = 2,
        AmountTooLow = 3
    }
    
    public Guid? DataGuid { get; set; }
    public Guid? CampaignGuid { get; set; }
    public Guid? TypeGuid { get; set; }
    public int? CreatorUserId { get; set; }
    public bool IsExpense { get; set; } = false;
    public Decimal? Amount { get; set; }
    public string? DataTitle { get; set; }
    public string? DataDescription { get; set; }
    public DateTime? DateCreated { get; set; }

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