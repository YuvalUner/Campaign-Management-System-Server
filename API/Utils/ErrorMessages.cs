using DAL.DbAccess;

namespace API.Utils;

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
    public const string UserAlreadyVerified = "User with this ID is already verified, this may require a report to the admin";
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
    public const string PermissionOrAuthorizationError = "Permission or authorization error - you do not have permission to do this";
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
    public const string EventNotAssociatedToCampaign = "Event is not associated to any campaign and can not be published";
    public const string AnnouncementTitleRequired = "Announcement title is required";
    public const string AnnouncementContentRequired = "Announcement content is required";
    public const string AnnouncementTitleTooLong = "Announcement title must be 100 characters or less";
    public const string AnnouncementContentTooLong = "Announcement content must be 4000 characters or less";
    public const string AnnouncementNotFound = "Announcement not found";
    public const string NotificationSettingsRequired = "Notification settings are required and can not both be false";
    public const string TooManyFinancialTypes = "Too many financial types in campaign - you may not have over 100";
    public const string FinancialTypeNotFound = "Financial type not found";
    public const string CanNotModifyThisType = "You can not modify this type in any way";

    public static string FormatErrorMessage(string message, CustomStatusCode customStatusCode)
    {
        return $"Error Num {(int)customStatusCode} - {message}";
    }
}