namespace RestAPIServices;

/// <summary>
/// A service for sending SMS messages to users.<br/>
/// Provides a layer of abstraction between the SMS provider and the rest of the application, and also 
/// handles all the formatting of the messages and formatting of the phone numbers.<br/>
/// Provides methods for sending structured messages that fit a certain format, such as a verification code or a
/// notification that a user joined a campaign, as well as a method for sending a custom message.
/// </summary>
public interface ISmsMessageSendingService
{
    /// <summary>
    /// Sends a message to the phone number that a user joined a campaign
    /// </summary>
    /// <param name="userName">Name of the user that joined the campaign.</param>
    /// <param name="campaignName">Name of the campaign the user joined.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a verification code to the phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="code">The code to send to the user.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendPhoneVerificationCodeAsync(string? phoneNumber, string code, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard role assigned message to the phone number
    /// </summary>
    /// <param name="roleName">Name of the role the user was assigned to.</param>
    /// <param name="campaignName">Name of the campaign they were assigned the role in.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendRoleAssignedSmsAsync(string? roleName, string? campaignName, string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard job assigned message to the phone number
    /// </summary>
    /// <param name="jobName">Name of the job.</param>
    /// <param name="jobStartTime">When the job begins.</param>
    /// <param name="jobEndTime">When the job ends.</param>
    /// <param name="location">Location of the job.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendJobAssignedSmsAsync(string? jobName, DateTime? jobStartTime, DateTime? jobEndTime, string? location,
        string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard job unassigned message to the phone number
    /// </summary>
    /// <param name="jobName">Name of the job.</param>
    /// <param name="location">Where the job was.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendJobUnAssignedSmsAsync(string? jobName, string? location, string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Allows to send a free text message to a phone number.
    /// To be used when none of the other templates apply, such as when the user wants to send a custom message.
    /// </summary>
    /// <param name="message">Free text, can be anything.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task<CallStatus> SendFreeTextSmsAsync(string? message, string phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard event participation message to the phone number.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendAddedEventParticipationSmsAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string? phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard event created message to the phone number.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="creatorName">Name of the person who created the event.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendEventCreatedForUserSmsAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string? creatorName,
        string? phoneNumber, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard event deleted message to the phone number.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="senderName">Name of the one who deleted the event.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendEventDeletedMessageAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? phoneNumber, string? senderName, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard event updated message to the phone number.
    /// If location, start time or end time are null, they will not be included in the message.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="senderName">Name of the one who updated the event.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendEventUpdatedMessageAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? phoneNumber, string? senderName, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard event published message to the phone number.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="eventLocation">Location or address of the event.</param>
    /// <param name="startTime">When the event starts.</param>
    /// <param name="endTime">When the event ends.</param>
    /// <param name="phoneNumber">Phone number to send to.</param>
    /// <param name="senderName">Name of the one who published the event, or the one who created it.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendEventPublishedMessageAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? phoneNumber, string? senderName, CountryCodes countryCode);

    /// <summary>
    /// Sends a standard announcement created message to the phone number.
    /// </summary>
    /// <param name="announcementTitle">The title of the announcement.</param>
    /// <param name="phoneNumber"></param>
    /// <param name="senderName">Name of the one who created and published the announcement.</param>
    /// <param name="countryCode">Country code for the user's provider, one of those defined in <see cref="CountryCodes"/></param>
    /// <returns></returns>
    Task SendAnnouncementPublishedMessageAsync(string? announcementTitle, string? phoneNumber,
        string? senderName, CountryCodes countryCode);
}