namespace DAL.Models;

/// <summary>
/// A model for a single row of the financial_types table.<br/>
/// </summary>
public class FinancialType
{
    private readonly int _maxFinancialTypeTitleLength = 100;
    private readonly int _maxFinancialTypeDescriptionLength = 300;
    
    /// <summary>
    /// A collection of codes to use to determine success or failure of a validation operation of this model.<br/>
    /// </summary>
    public enum ValidationFailureCodes
    {
        Ok = 0,
        TitleTooLong = 1,
        DescriptionTooLong = 2
    }

    public Guid? CampaignGuid { get; set; }
    public string? TypeName { get; set; }
    public string? TypeDescription { get; set; }
    
    /// <summary>
    /// A unique identifier for the financial type.<br/>
    /// Can and should be exposed to the user.<br/>
    /// </summary>
    public Guid? TypeGuid { get; set; }
    
    /// <summary>
    /// A method for validating user input when creating or updating a financial type.<br/>
    /// </summary>
    /// <param name="isCreation">Set to true when creating a new financial type, false otherwise. Modifies the method's
    /// behavior to fit these cases.</param>
    /// <returns>A <see cref="ValidationFailureCodes"/> value - Ok if everything is fine, one of the error codes, depending on
    /// the first error found, otherwise.</returns>
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