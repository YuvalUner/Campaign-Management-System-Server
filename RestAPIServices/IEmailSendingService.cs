namespace RestAPIServices;

/// <summary>
/// A service for sending emails to users.<br/>
/// Provides a layer of abstraction between the email provider and the rest of the application, and also
/// handles all the formatting of the messages and formatting of the email addresses.<br/>
/// Provides methods for sending structured messages that fit a certain format.
/// </summary>
public interface IEmailSendingService
{
    /// <summary>
    /// Sends a standard user joined notification to the email address
    /// </summary>
    /// <param name="userName">Name of the user that joined a campaign.</param>
    /// <param name="campaignName">Name of the campaign the user joined.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <returns></returns>
    Task SendUserJoinedEmailAsync(string? userName, string? campaignName, string? emailTo);

    /// <summary>
    /// Sends a standard role assigned notification to the email address
    /// </summary>
    /// <param name="roleName">Name of the role the user was assigned.</param>
    /// <param name="campaignName">Name of the campaign the user joined.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <returns></returns>
    Task SendRoleAssignedEmailAsync(string? roleName, string? campaignName, string? emailTo);

    /// <summary>
    /// Sends a standard job assigned notification to the email address
    /// </summary>
    /// <param name="jobName">Name of the job.</param>
    /// <param name="jobStartTime">When the job begins.</param>
    /// <param name="jobEndTime">When the job ends.</param>
    /// <param name="location">Location of the job.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <returns></returns>
    Task SendJobAssignedEmailAsync(string? jobName, DateTime? jobStartTime, DateTime? jobEndTime, string? location,
        string? emailTo);

    /// <summary>
    /// Sends a standard job unassigned notification to the email address
    /// </summary>
    /// <param name="jobName">Name of the job.</param>
    /// <param name="location">Location of the job.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <returns></returns>
    Task SendJobUnAssignedEmailAsync(string? jobName, string? location, string? emailTo);

    /// <summary>
    /// Sends a standard event participation notification to the email address.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <param name="senderName">Name of the campaign or user sending the email.</param>
    /// <returns></returns>
    Task SendAddedEventParticipationEmailAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string? emailTo, string? senderName);

    /// <summary>
    /// Sends a standard event created notification to the email address.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="creatorName">Name of the creator of the event.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <param name="senderName">Name of the campaign or user sending the email.</param>
    /// <returns></returns>
    Task SendEventCreatedForUserEmailAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string? creatorName, string? emailTo,
        string? senderName);

    /// <summary>
    /// Sends a standard event deleted notification to the email address.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <param name="senderName">Name of the campaign or user sending the email.</param>
    /// <returns></returns>
    Task SendEventDeletedEmailAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? emailTo, string? senderName);

    /// <summary>
    /// Sends a standard event updated notification to the email address.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <param name="senderName">Name of the campaign or user sending the email.</param>
    /// <returns></returns>
    Task SendEventUpdatedEmailAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? emailTo, string? senderName);

    /// <summary>
    /// Sends a standard event published notification to the email address.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <param name="senderName">Name of the campaign or user sending the email.</param>
    /// <returns></returns>
    Task SendEventPublishedEmailAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? emailTo, string? senderName);


    /// <summary>
    /// Sends a standard announcement created notification to the email address.
    /// </summary>
    /// <param name="announcementTitle">Title of the announcement that was created.</param>
    /// <param name="announcementContent">Content of the announcement, to be included in the email's body.</param>
    /// <param name="emailTo">Email address to send the message to.</param>
    /// <param name="senderName">Name of the campaign or user sending the email.</param>
    /// <returns></returns>
    Task SendAnnouncementPublishedEmailAsync(string? announcementTitle, string? announcementContent,
        string? emailTo, string? senderName);
}