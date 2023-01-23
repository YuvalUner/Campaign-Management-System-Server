using DAL.DbAccess;
using DAL.Services.Interfaces;
using RestAPIServices;

namespace DAL.Services.Implementations;

public class NotificationsService : INotificationsService
{
    private readonly IGenericDbAccess _dbAccess;
    private readonly IEmailSendingService _emailSendingService;
    private readonly ISmsMessageSendingService _smsMessageSendingService;

    public NotificationsService(IGenericDbAccess dbAccess, IEmailSendingService emailSendingService,
        ISmsMessageSendingService smsMessageSendingService)
    {
        _dbAccess = dbAccess;
        _emailSendingService = emailSendingService;
        _smsMessageSendingService = smsMessageSendingService;
    }

    public async Task SendUserJoinedEmailAsync(string? userName, string? campaignName, string emailTo)
    {
        var subject = "Welcome to the campaign!";
        var message = $"Hi {userName}, you have joined the campaign {campaignName}!";
        await _emailSendingService.SendEmailAsync(emailTo, subject, message);
    }
}