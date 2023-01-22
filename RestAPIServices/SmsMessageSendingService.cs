using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telesign;

namespace RestAPIServices;

public interface ISmsMessageSendingService
{
    Task SendSmsMessageAsync(string phoneNumber, string message);
    Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber);
}

public class SmsMessageSendingService : ISmsMessageSendingService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsMessageSendingService> _logger;
    
    public SmsMessageSendingService(IConfiguration configuration, ILogger<SmsMessageSendingService> logger)
    {
        _configuration = configuration;
        _logger = logger;
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
        await SendSmsMessageAsync(phoneNumber, message);
        
    }
}