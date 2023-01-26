namespace DAL.Models;

public class PhoneVerificationCode
{
    public string? VerificationCode { get; set; }
    public string? PhoneNumber { get; set; }
    
    public DateTime? Expires { get; set; }
}