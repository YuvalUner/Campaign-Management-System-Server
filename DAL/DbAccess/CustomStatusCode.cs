namespace DAL.DbAccess;

/// <summary>
/// Custom status codes for the application.
/// Holds both application errors and SQL errors.
/// </summary>
public enum CustomStatusCode
{
    // 0 is the default return value for stored procedures, so we use it for success
    // Outside of tests, this should never be used.
    // However, as it is needed to be defined for the enum to work, it is here and removing it will break literally everything.
    Ok = 0,
    // 1 and above are custom error codes, relating to specific errors that can happen within the app.
    // Some relate to SQL, but are not SQL errors but rather errors due to user input or other logic
    IdAlreadyExistsWhenVerifyingInfo = 1,
    ValueCanNotBeNull = 2,
    NameCanNotBeBuiltIn = 3,
    ValueNotFound = 4,
    PhoneNumberNotFound = 5,
    VerificationStatusError = 6,
    VerificationCodeExpired = 7,
    AuthorizationError = 8,
    PermissionError = 9,
    PermissionOrAuthorizationError = 10,
    CityNameRequired = 11,
    AlreadyVerified = 12,
    NotLoggedIn = 13,
    IllegalValue = 14,
    ValueNullOrEmpty = 15,
    
    // 50000 and above are SQL errors, meant to match requirement of throwing between 50000 and 2147483647
    // These are for errors that would have been thrown by the database if not caught by the stored procedure
    // and returned as a custom error code
    DuplicateKey = 50001,
    ForeignKeyViolation = 50002,
    CannotInsertNull = 50003,
    TooManyEntries = 50004,
    CannotInsertDuplicateUniqueIndex = 50005,
    RoleNotFound = 50006,
    UserNotFound = 50007,
    RoleAlreadyExists = 50008,
    PermissionDoesNotExist = 50009,
    UserAlreadyHasPermission = 50010,
    CityNotFound = 50011,
    JobNotFound = 50012,
    JobTypeNotFound = 50013,
    JobFullyManned = 50014,
    UserNotInCampaign = 50015,
    ParameterMustNotBeNullOrEmpty = 50016,
    CampaignNotFound = 50017,
    EventNotFound = 50018,
}