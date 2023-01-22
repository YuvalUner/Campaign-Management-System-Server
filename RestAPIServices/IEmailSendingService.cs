namespace RestAPIServices;

public interface IEmailSendingService
{
    Task SendEmailAsync(string emailTo, string subject, string message);
    Task SendUserJoinedEmailAsync(string? userName, string? campaignName, string emailTo);
}