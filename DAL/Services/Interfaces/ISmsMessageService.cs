using DAL.Models;
using RestAPIServices;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for managing, sending and logging SMS messages.
/// </summary>
public interface ISmsMessageService
{
    /// <summary>
    /// Adds a message to the database
    /// </summary>
    /// <param name="smsSendingParams">An instance of <see cref="SmsSendingParams"/> with all of the required properties
    /// filled in.</param>
    /// <param name="countryCode">A country code from <see cref="CountryCodes"/>.</param>
    /// <returns>Tuple containing the new message's Guid, to be returned to the client, and the new message's id,
    /// to be used with SendSmsMessageToMultiplePhones</returns>
    Task<(Guid, int)> AddSmsMessage(SmsSendingParams smsSendingParams, CountryCodes countryCode);

    /// <summary>
    /// Sends a message to multiple phone numbers and adds the message being sent to the database
    /// Generally, this method should be called after AddSmsMessage, using the newMessageId returned from it.
    /// This method should NOT be awaited - it should be called in a fire-and-forget manner.
    /// </summary>
    /// <param name="smsSendingParams">An instance of <see cref="SmsMessageSendingService"/> with all of the required
    /// properties filled in.</param>
    /// <param name="newMessageId">id of the new message, received from <see cref="AddSmsMessage"/> as the return value.</param>
    /// <param name="countryCode">A country code from <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendSmsMessageToMultiplePhones(SmsSendingParams smsSendingParams, int newMessageId, CountryCodes countryCode);

    /// <summary>
    /// Gets the base SMS logs for a campaign.<br/>
    /// Base SMS logs contain the message's content, the message's date, the number of times the message was sent,
    /// and who sent the message.<br/>
    /// They do not contain the message's recipients or whether the message was sent successfully or not to each of them.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get the logs for.</param>
    /// <returns>A list of <see cref="SmsLogResult"/>, each item containing info about a single SMS sent.</returns>
    Task<IEnumerable<SmsLogResult>> GetBaseSmsLogs(Guid campaignGuid);

    /// <summary>
    /// Gets the SMS details logs for a message.<br/>
    /// Returns message's recipients and whether the message was sent successfully or not to each of them.
    /// </summary>
    /// <param name="messageGuid">Guid of the specific message to get the logs for.</param>
    /// <returns>A list of <see cref="SmsDetailsLogResult"/>, each item containing info about a user who was sent the SMS.</returns>
    Task<IEnumerable<SmsDetailsLogResult>> GetSmsDetailsLog(Guid messageGuid);
}