using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telesign;

namespace RestAPIServices;


public class SmsMessageSendingService : ISmsMessageSendingService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsMessageSendingService> _logger;
    
    public SmsMessageSendingService(IConfiguration configuration, ILogger<SmsMessageSendingService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    private static string CleanPhoneNumber(string? phoneNumber)
    {
        if (phoneNumber == null)
            return "";
        return phoneNumber.Replace(" ", "").Replace("-", "");
    }
    
    public static string MakePhoneNumberForIsrael(string? phoneNumber)
    {
        if (phoneNumber == null)
            return "";
        phoneNumber = CleanPhoneNumber(phoneNumber);
        // Apparently, Telesign wants the number with the 0.
        // if (phoneNumber.StartsWith("0"))
        //     phoneNumber = phoneNumber.Substring(1);
        if (phoneNumber.StartsWith("972"))
            return phoneNumber;
        return "972" + phoneNumber;
    }

    public async Task SendSmsMessageAsync(string phoneNumber, string message)
    {
        try
        {
            MessagingClient messagingClient = new MessagingClient(_configuration["SmsConfiguration:Telesign:CustomerId"],
                _configuration["SmsConfiguration:Telesign:ApiKey"]);
            RestClient.TelesignResponse telesignResponse = await messagingClient.MessageAsync(phoneNumber, message, "ARN");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending sms message");
        }
    }

    public async Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber)
    {
        string message = $"User {userName} joined campaign {campaignName}";
        phoneNumber = MakePhoneNumberForIsrael(phoneNumber);
        await SendSmsMessageAsync(phoneNumber, message);
        
    }
    
    public async Task SendPhoneVerificationCodeAsync(string? phoneNumber, string code)
    {
        string message = $"Your verification code is {code}";
        phoneNumber = MakePhoneNumberForIsrael(phoneNumber);
        await SendSmsMessageAsync(phoneNumber, message);
    }
}