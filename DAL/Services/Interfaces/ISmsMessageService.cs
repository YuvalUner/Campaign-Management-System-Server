using DAL.Models;
using RestAPIServices;

namespace DAL.Services.Interfaces;

public interface ISmsMessageService
{
    /// <summary>
    /// Adds a message to the database
    /// </summary>
    /// <param name="smsSendingParams"></param>
    /// <param name="countryCode"></param>
    /// <returns>Tuple containing the new message's Guid, to be returned to the client, and the new message's id,
    /// to be used with SendSmsMessageToMultiplePhones</returns>
    Task<(Guid, int)> AddSmsMessage(SmsSendingParams smsSendingParams, CountryCodes countryCode);

    /// <summary>
    /// Sends a message to multiple phone numbers and adds the message being sent to the database
    /// Generally, this method should be called after AddSmsMessage, using the newMessageId returned from it.
    /// This method should NOT be awaited - it should be called in a fire-and-forget manner.
    /// </summary>
    /// <param name="smsSendingParams"></param>
    /// <param name="newMessageId"></param>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    Task SendSmsMessageToMultiplePhones(SmsSendingParams smsSendingParams, int newMessageId, CountryCodes countryCode);
}