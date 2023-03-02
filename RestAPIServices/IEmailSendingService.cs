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

    /// <summary>
    /// Sends a standard event participation notification to the email address.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventLocation"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="emailTo"></param>
    /// <param name="senderName"></param>
    /// <returns></returns>
    Task SendAddedEventParticipationEmailAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string? emailTo, string? senderName);

    /// <summary>
    /// Sends a standard event created notification to the email address.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventLocation"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="creatorName"></param>
    /// <param name="emailTo"></param>
    /// <param name="senderName"></param>
    /// <returns></returns>
    Task SendEventCreatedForUserEmailAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string creatorName, string? emailTo,
        string? senderName);
}