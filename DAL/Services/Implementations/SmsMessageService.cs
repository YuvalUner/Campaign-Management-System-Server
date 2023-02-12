using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;
using RestAPIServices;

namespace DAL.Services.Implementations;

public class SmsMessageService: ISmsMessageService
{
    private readonly IGenericDbAccess _dbAccess;
    private readonly ISmsMessageSendingService _smsMessageSendingService;

    public SmsMessageService(IGenericDbAccess dbAccess, ISmsMessageSendingService smsMessageSendingService)
    {
        _dbAccess = dbAccess;
        _smsMessageSendingService = smsMessageSendingService;
    }

    public async Task<(Guid, int)> AddSmsMessage(SmsSendingParams smsSendingParams, CountryCodes countryCode)
    {
        // Add the message to the database
        var smsAddingParams = new DynamicParameters(new
        {
            smsSendingParams.CampaignGuid,
            smsSendingParams.SenderId,
            smsSendingParams.MessageContents
        });
        smsAddingParams.Add("newMessageGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        smsAddingParams.Add("newMessageId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await _dbAccess.ModifyData(StoredProcedureNames.AddSmsMessage, smsAddingParams);
        
        var newMessageGuid = smsAddingParams.Get<Guid>("newMessageGuid");
        var newMessageId = smsAddingParams.Get<int>("newMessageId");
        
        return (newMessageGuid, newMessageId);
    }

    public async Task SendSmsMessageToMultiplePhones(SmsSendingParams smsSendingParams, int newMessageId, CountryCodes countryCode)
    {
        foreach (var phoneNumber in smsSendingParams.PhoneNumbers)
        {
            // Send the message and get whether it was probably successful or definitely failed
            CallStatus res = await _smsMessageSendingService.SendFreeTextSmsAsync(smsSendingParams.MessageContents, phoneNumber, countryCode);
            
            // Add the message being sent to the database, regardless of whether it was successful or not
            // This is done for both logging purposes and for potential billing purposes
            var smsSendingParamsDb = new DynamicParameters(new
            {
                messageId = newMessageId,
                phoneNumber,
                isSuccess = res
            });
            await _dbAccess.ModifyData(StoredProcedureNames.AddSmsMessageSent, smsSendingParamsDb);
        }
    }
}