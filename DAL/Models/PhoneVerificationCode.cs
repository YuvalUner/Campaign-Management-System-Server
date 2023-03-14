namespace DAL.Models;

/// <summary>
/// A model for a phone verification code, that matches most of the phone_verification_codes table.<br/>
/// </summary>
public class PhoneVerificationCode
{
    public string? VerificationCode { get; set; }
    public string? PhoneNumber { get; set; }
    
    public DateTime? Expires { get; set; }
}