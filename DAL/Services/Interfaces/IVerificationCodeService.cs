using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for creating, getting and approving verification codes for users.
/// </summary>
public interface IVerificationCodeService
{
    /// <summary>
    /// Creates a verification code for a user and stores it in the database along with their phone number.
    /// </summary>
    /// <param name="userId">The user id of the user to add a verification code for.</param>
    /// <param name="code">An instance of <see cref="PhoneVerificationCode"/> with the phone number and code in it.</param>
    /// <returns></returns>
    Task CreateVerificationCode(int? userId, PhoneVerificationCode code);
    
    /// <summary>
    /// Gets a user's verification code and phone number from the database.
    /// </summary>
    /// <param name="userId">The user id to get the code for.</param>
    /// <returns>An instance of <see cref="PhoneVerificationCode"/> with the code in it.</returns>
    Task<PhoneVerificationCode?> GetVerificationCode(int? userId);
    
    /// <summary>
    /// Approves the verification code for a user and removes it from the database, as well as stores the phone number.
    /// </summary>
    /// <param name="userId">The user id of the user for which the code was approved.</param>
    /// <param name="phoneNumber">The phone number to store in the user's info.</param>
    /// <returns></returns>
    Task ApproveVerificationCode(int? userId, string? phoneNumber);
}