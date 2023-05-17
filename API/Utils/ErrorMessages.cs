using DAL.DbAccess;

namespace API.Utils;

/// <summary>
/// A util for storing and formatting error messages that are used in multiple places.<br/>
/// Used to avoid having the same error message in multiple places, and to avoid typos.<br/>
/// To use, use the method <see cref="FormatErrorMessage"/> within this class, and pass it the name of the error message
/// you want to use + the status code of the error from <see cref="CustomStatusCode"/>.<br/>
/// </summary>
public static class ErrorMessages
{
    public const string JobNameRequired = "Job name is required";
    public const string UserNotFound = "User not found";
    public const string RoleNotFound = "Role not found";
    public const string RoleAlreadyExists = "Role already exists";
    public const string UserAlreadyExists = "User already exists";
    public const string JobTypeAlreadyExists = "Job type already exists";
    public const string JobTypeNotFound = "Job type not found";
    public const string TooManyRoles = "Too many roles in campaign";

    public const string UserAlreadyVerified =
        "User with this ID is already verified, this may require a report to the admin";

    public const string PermissionDoesNotExist = "Permission does not exist";
    public const string AlreadyHasPermission = "User already has this permission";
    public const string CityNotFound = "City not found";
    public const string RoleNameRequired = "Role name is required";
    public const string NameMustNotBeBuiltIn = "Name must not be a built-in name";
    public const string RequestedValueNotFound = "Requested value not found";
    public const string VerificationStatusError = "Verification status error - you must be verified to do this";
    public const string PhoneNumberNotFound = "Phone number not found";
    public const string VerificationCodeNotFound = "Verification code not found, please request a new one";
    public const string VerificationCodeExpired = "Verification code expired, please request a new one";
    public const string AuthorizationError = "Authorization error - you are not authorized to do this";
    public const string PermissionError = "Permission error - you do not have permission to do this";
    public const string PermissionOrAuthorizationError =
        "Permission or authorization error - you do not have permission to do this";

    public const string CityNameRequired = "City name is required when filtering for non-municipal campaigns";
    public const string CampaignNameRequired = "Campaign name is required";
    public const string CampaignNotFound = "Campaign not found";
    public const string JobNotFound = "Job not found";
    public const string CampaignNameOrCityNameRequired = "Campaign name or city name is required";
    public const string JobTypeRequired = "Job type is required";
    public const string TooManyJobTypes = "Too many job types in campaign";
    public const string DuplicateVerification = "You can not verify a user more than once";
    public const string VerificationFailed = "Verification failed, please check your info and try again";
    public const string AlreadyAMember = "You are already a member of this campaign";
    public const string RoleAlreadyAssigned = "Role already assigned to user";
    public const string PermissionNotFound = "Permission not found";
    public const string UserAlreadyHasPermission = "User already has this permission";
    public const string NotLoggedIn = "You are not logged in";
    public const string JobRequiresPeople = "Number of people required for job must be greater than 0";
    public const string CanAlreadyAssignToJob = "User can already assign to this job";
    public const string CanAlreadyAssignToJobType = "User can already assign to this job type";
    public const string JobFullyManned = "Job is fully manned, can not assign more people";
    public const string AlreadyAssignedToJob = "User is already assigned to this job";
    public const string NoPermissionToAssignToJob = "User does not have permission to assign to this job";
    public const string EmailNullOrEmpty = "Email can not be null or empty";
    public const string SalaryNullOrEmpty = "Salary can not be null or empty";
    public const string PreferencesNullOrEmpty = "Preferences can not be null or empty";
    public const string TimeframeMustBeProvided = "Timeframe must be provided";
    public const string PhoneNumbersRequired = "List of phone numbers must not be empty";
    public const string MessageContentRequired = "Message content must not be empty";
    public const string EventNameIsRequired = "Event name is required";
    public const string EventNotFound = "Event not found. Please check the event ID and try again";
    public const string EventAlreadyFull = "Event is already full, can not add more people";
    public const string AlreadyParticipating = "You are already participating in this event";
    public const string AlreadyWatcher = "You are already watching this event";
    public const string EmailRequired = "Email is required";
    public const string MaxAttendeesNotNullOrZero = "Max attendees must be greater than 0";
    public const string ManagerAlreadyExists = "User is already your manager";
    public const string EventAlreadyPublished = "Event is already published";
    public const string EventNotAssociatedToCampaign =
        "Event is not associated to any campaign and can not be published";
    
    public const string AnnouncementTitleRequired = "Announcement title is required";
    public const string AnnouncementContentRequired = "Announcement content is required";
    public const string AnnouncementTitleTooLong = "Announcement title must be 100 characters or less";
    public const string AnnouncementContentTooLong = "Announcement content must be 4000 characters or less";
    public const string AnnouncementNotFound = "Announcement not found";
    public const string NotificationSettingsRequired = "Notification settings are required and can not both be false";
    public const string TooManyFinancialTypes = "Too many financial types in campaign - you may not have over 100";
    public const string FinancialTypeNotFound = "Financial type not found";
    public const string CanNotModifyThisType = "You can not modify this type in any way";
    public const string InvalidFinancialTypeName = "Financial type name must not be empty or over 100 characters";
    public const string InvalidFinancialTypeDescription = "Financial type description must not be over 300 characters";
    public const string FinancialDataTitleTooLong = "Financial data title must not be over 50 characters";
    public const string FinancialDataDescriptionTooLong = "Financial data description must not be over 500 characters";
    public const string IllegalAmount = "Amount must be greater than 0";
    public const string FinancialDataNotFound = "Financial data not found";
    public const string UnknownError =
        "Unknown error, someone on the dev team forgot to add an error message for this situation and just stuck this here. Please contact us.";
    
    public const string NoBallotFound =
        "Ballot not found. If you are a minor, this may be because you are not old enough to vote." +
        "Otherwise, the error may be on our end or the civil registry's end. Please contact us.";
    
    public const string NotInCampaign = "You are not a member of this campaign";
    public const string VerificationCodeError = "Verification code error - please check your code and try again";
    public const string LedgerNotFound = "The requested ledger was not found";
    public const string IdentifierMissing = "You must provide an identifier for the ledger";
    public const string RowAlreadyExists = "A row with the same identifier already exists";
    public const string LedgerRowNotFound = "The requested ledger row was not found";
    public const string NoFileProvided = "No file was provided";
    public const string InvalidFile = "The provided file is invalid - please check the file and try again";
    public const string OpponentNameRequired = "Opponent name is required";

    /// <summary>
    /// Formats an error message with a custom status code, such that it can be returned to the client in a consistent way.
    /// </summary>
    /// <param name="message">The string describing the error. Should be any of the above strings.</param>
    /// <param name="customStatusCode">A status code from <see cref="CustomStatusCode"/>, so that the error can be
    /// easily tracked.</param>
    /// <returns>An error string for the error that occurred, with a uniform structure for all errors.</returns>
    public static string FormatErrorMessage(string message, CustomStatusCode customStatusCode)
    {
        return $"Error Num {(int)customStatusCode} - {message}";
    }
}