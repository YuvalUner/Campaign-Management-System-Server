namespace RestAPIServices;

public interface IEmailSendingService
{
    Task SendUserJoinedEmailAsync(string? userName, string? campaignName, string emailTo);

    Task SendRoleAssignedEmailAsync(string? roleName, string? campaignName, string? emailTo);
}