namespace RestAPIServices;

public interface ISmsMessageSendingService
{
    Task SendSmsMessageAsync(string phoneNumber, string message);
    Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber);
}