namespace DAL.Models;

public class FinancialType
{
    private readonly int _maxFinancialTypeTitleLength = 100;
    private readonly int _maxFinancialTypeDescriptionLength = 300;
    
    public enum ValidationFailureCodes
    {
        Ok = 0,
        TitleTooLong = 1,
        DescriptionTooLong = 2
    }

    public Guid? CampaignGuid { get; set; }
    public string? TypeName { get; set; }
    public string? TypeDescription { get; set; }
    public Guid? TypeGuid { get; set; }
    
    public ValidationFailureCodes VerifyLegalValues(bool isCreation = true)
    {
        // If we're creating a new financial type, the title must be non-null, non-empty and not too long
        // If we're updating an existing financial type, the title must be either null or non-empty and not too long
        // The length == 0 check is to prevent the user from setting the title to an empty string, and was 
        // added after the initial implementation of this method, making the return code in that specific case
        // inconsistent, but it's so minor that changing it would be more trouble than it's worth.
        if ((isCreation && (string.IsNullOrWhiteSpace(TypeName) 
                           || TypeName.Length > _maxFinancialTypeTitleLength)) 
            || (!isCreation && TypeName != null && (TypeName.Length > _maxFinancialTypeTitleLength || TypeName.Length == 0)))
        {
            return ValidationFailureCodes.TitleTooLong;
        }
        
        if (TypeDescription != null && TypeDescription.Length > _maxFinancialTypeDescriptionLength)
        {
            return ValidationFailureCodes.DescriptionTooLong;
        }
        
        return ValidationFailureCodes.Ok;
    }
    
}