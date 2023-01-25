namespace RestAPIServices;

public interface ISmsMessageSendingService
{
    
    /// <summary>
    /// Generic method to send a message to a phone number
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    Task SendSmsMessageAsync(string phoneNumber, string message);
    
    /// <summary>
    /// Sends a message to the phone number that a user joined a campaign
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="campaignName"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber);

    /// <summary>
    /// Sends a verification code to the phone number
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task SendPhoneVerificationCodeAsync(string? phoneNumber, string code);
}