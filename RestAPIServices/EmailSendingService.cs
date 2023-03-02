using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RestAPIServices;

public class EmailSendingService : IEmailSendingService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSendingService> _logger;

    public EmailSendingService(IConfiguration configuration, ILogger<EmailSendingService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task SendEmailAsync(string emailTo, string subject, string message, string? senderName = null)
    {

        try
        {
            SmtpClient smtpServer = new SmtpClient(_configuration["EmailConfiguration:Gmail:SmtpServer"],
                Convert.ToInt32(_configuration["EmailConfiguration:Gmail:SmtpPort"]));
            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage email = new MailMessage();
            
            if (senderName != null)
                email.From = new MailAddress(_configuration["EmailConfiguration:Gmail:Email"], senderName);
            else
            {
                email.From = new MailAddress(_configuration["EmailConfiguration:Gmail:Email"]);
            }
            email.To.Add(emailTo);
            email.Subject = subject;
            email.Body = message;
            
            smtpServer.Timeout = 5000;
            smtpServer.EnableSsl = true;
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new NetworkCredential(_configuration["EmailConfiguration:Gmail:Email"],
                _configuration["EmailConfiguration:Gmail:Password"]);
            smtpServer.SendAsync(email, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending email");
        }

    }
    
    public async Task SendUserJoinedEmailAsync(string? userName, string? campaignName, string emailTo)
    {
        string subject = $"User joined campaign";
        string message = $"User {userName} joined campaign {campaignName}";
        await SendEmailAsync(emailTo, subject, message, campaignName);
    }

    public async Task SendRoleAssignedEmailAsync(string? roleName, string? campaignName, string? emailTo)
    {
        string subject = $"Role assigned";
        string message = $"Role {roleName} was assigned to you in campaign {campaignName}";
        await SendEmailAsync(emailTo, subject, message, campaignName);
    }
    
    public async Task SendJobAssignedEmailAsync(string? jobName, DateTime? jobStartTime, DateTime? jobEndTime, string? location, string? emailTo)
    {
        string subject = $"Job assigned";
        string message = $"You were assigned the job {jobName} from {jobStartTime} to {jobEndTime} at {location}";
        await SendEmailAsync(emailTo, subject, message, jobName);
    }
    
    public async Task SendJobUnAssignedEmailAsync(string? jobName, string? location, string? emailTo)
    {
        string subject = $"Job unassigned";
        string message = $"You were unassigned from the job {jobName} at {location}";
        await SendEmailAsync(emailTo, subject, message, jobName);
    }
    
    public async Task SendAddedEventParticipationEmailAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime,  string? emailTo, string? senderName)
    {
        string subject = $"Event participation added";
        string message;
        if (startTime != null && endTime != null)
        {
            message = $"You were added to the event {eventName} at {eventLocation} from {startTime} to {endTime}";
        }
        else if (startTime != null)
        {
            message = $"You were added to the event {eventName} at {eventLocation} starting at {startTime}";
        }
        else if (endTime != null)
        {
            message = $"You were added to the event {eventName} at {eventLocation} ending at {endTime}";
        }
        else
        {
            message = $"You were added to the event {eventName} at {eventLocation}";
        }
        await SendEmailAsync(emailTo, subject, message, senderName);
    }

    public async Task SendEventCreatedForUserEmailAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string creatorName, string? emailTo,
        string? senderName)
    {
        string subject = $"Event created and added to your schedule";
        string message;
        if (startTime != null && endTime != null)
        {
            message = $"Event {eventName} at {eventLocation} from {startTime} to {endTime} was created by {creatorName} and added to your schedule";
        }
        else if (startTime != null)
        {
            message = $"Event {eventName} at {eventLocation} starting at {startTime} was created by {creatorName} and added to your schedule";
        }
        else if (endTime != null)
        {
            message = $"Event {eventName} at {eventLocation} ending at {endTime} was created by {creatorName} and added to your schedule";
        }
        else
        {
            message = $"Event {eventName} at {eventLocation} was created by {creatorName} and added to your schedule";
        }
        await SendEmailAsync(emailTo, subject, message, senderName);
    }

    public async Task SendEventDeletedEmailAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? emailTo, string? senderName)
    {
        string subject = $"Event deleted";
        string message;
        if (startTime != null && endTime != null)
        {
            message = $"Event {eventName} at {eventLocation} from {startTime} to {endTime} was deleted";
        }
        else if (startTime != null)
        {
            message = $"Event {eventName} at {eventLocation} starting at {startTime} was deleted";
        }
        else if (endTime != null)
        {
            message = $"Event {eventName} at {eventLocation} ending at {endTime} was deleted";
        }
        else
        {
            message = $"Event {eventName} at {eventLocation} was deleted";
        }
        await SendEmailAsync(emailTo, subject, message, senderName);
    }

    public async Task SendEventUpdatedEmailAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? emailTo, string? senderName)
    {
        string subject = $"Event updated";
        string message = $"Event name: {eventName} \n";
        if (eventLocation != null)
        {
            message += $"Location moved to: {eventLocation} \n";
        }
        if (startTime != null)
        {
            message += $"Start time changed to: {startTime} \n";
        }
        if (endTime != null)
        {
            message += $"End time changed to: {endTime} \n";
        }
        await SendEmailAsync(emailTo, subject, message, senderName);
    }
}