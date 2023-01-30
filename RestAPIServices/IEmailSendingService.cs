namespace RestAPIServices;

public interface IEmailSendingService
{
    /// <summary>
    /// Sends a standard user joined notification to the email address
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="campaignName"></param>
    /// <param name="emailTo"></param>
    /// <returns></returns>
    Task SendUserJoinedEmailAsync(string? userName, string? campaignName, string emailTo);

    /// <summary>
    /// Sends a standard role assigned notification to the email address
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="campaignName"></param>
    /// <param name="emailTo"></param>
    /// <returns></returns>
    Task SendRoleAssignedEmailAsync(string? roleName, string? campaignName, string? emailTo);

    /// <summary>
    /// Sends a standard job assigned notification to the email address
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="jobStartTime"></param>
    /// <param name="jobEndTime"></param>
    /// <param name="location"></param>
    /// <param name="emailTo"></param>
    /// <returns></returns>
    Task SendJobAssignedEmailAsync(string? jobName, DateTime? jobStartTime, DateTime? jobEndTime, string? location,
        string? emailTo);

    /// <summary>
    /// Sends a standard job unassigned notification to the email address
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="location"></param>
    /// <param name="emailTo"></param>
    /// <returns></returns>
    Task SendJobUnAssignedEmailAsync(string? jobName, string? location, string? emailTo);
}