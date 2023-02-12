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

    /// <summary>
    /// Sends a standard job assigned message to the phone number
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="jobStartTime"></param>
    /// <param name="jobEndTime"></param>
    /// <param name="location"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task SendJobAssignedSmsAsync(string? jobName, DateTime? jobStartTime, DateTime? jobEndTime, string? location,
        string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard job unassigned message to the phone number
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="location"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task SendJobUnAssignedSmsAsync(string? jobName, string? location, string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Allows to send a free text message to a phone number.
    /// To be used when none of the other templates apply, such as when the user wants to send a custom message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task<CallStatus> SendFreeTextSmsAsync(string? message, string phoneNumber, CountryCodes countryCode);
}