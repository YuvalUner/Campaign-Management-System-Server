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

    public async Task SendEmailAsync(string emailTo, string subject, string message, string? senderName = null)
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
}