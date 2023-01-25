using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IVerificationCodeService
{
    Task CreateVerificationCode(int? userId, PhoneVerificationCode code);
    Task<PhoneVerificationCode?> GetVerificationCode(int? userId);
    Task ApproveVerificationCode(int? userId, string? phoneNumber);
}