using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IVerificationCodeService
{
    /// <summary>
    /// Creates a verification code for a user and stores it in the database along with their phone number.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task CreateVerificationCode(int? userId, PhoneVerificationCode code);
    
    /// <summary>
    /// Gets a user's verification code and phone number from the database.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<PhoneVerificationCode?> GetVerificationCode(int? userId);
    
    /// <summary>
    /// Approves the verification code for a user and removes it from the database, as well as stores the phone number.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task ApproveVerificationCode(int? userId, string? phoneNumber);
}