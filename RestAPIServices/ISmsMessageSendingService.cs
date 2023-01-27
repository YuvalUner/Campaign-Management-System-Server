namespace RestAPIServices;

public interface ISmsMessageSendingService
{

    /// <summary>
    /// Sends a message to the phone number that a user joined a campaign
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="campaignName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a verification code to the phone number
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="code"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task SendPhoneVerificationCodeAsync(string? phoneNumber, string code, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard role assigned message to the phone number
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="campaignName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task SendRoleAssignedSmsAsync(string? roleName, string? campaignName, string phoneNumber, CountryCodes countryCode);
}