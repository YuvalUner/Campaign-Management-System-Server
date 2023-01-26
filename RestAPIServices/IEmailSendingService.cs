namespace RestAPIServices;

public interface IEmailSendingService
{
    Task SendEmailAsync(string emailTo, string subject, string message, string? senderName = null);
    Task SendUserJoinedEmailAsync(string? userName, string? campaignName, string emailTo);
}