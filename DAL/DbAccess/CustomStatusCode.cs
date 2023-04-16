namespace DAL.DbAccess;

/// <summary>
/// Custom status codes for the application.
/// Holds both application errors and SQL errors.
/// </summary>
public enum CustomStatusCode
{
    ///<summary>
    /// 0 is the default return value for stored procedures, so we use it for success.<br/>
    /// Outside of tests, this should never be used.<br/>
    /// However, as it is needed to be defined for the enum to work, it is here and removing it will break literally everything.
    /// </summary>
    Ok = 0,
    // 1 and above are custom error codes, relating to specific errors that can happen within the app.
    // Some relate to SQL, but are not SQL errors but rather errors due to user input or other logic

    /// <summary>
    /// Error code used when a user tries to verify again with the same Id number.<br/>
    /// Best case - this is due to a user sending requests not via the client, or an issue in the client.<br/>
    /// Worst case: someone had their Id number stolen.
    /// </summary>
    IdAlreadyExistsWhenVerifyingInfo = 1,

    /// <summary>
    /// Status code for when a value is null or empty, but should not be.
    /// </summary>
    ValueCanNotBeNull = 2,

    /// <summary>
    /// Status code for when a user tries to change or create something with a name that is a built-in name.
    /// </summary>
    NameCanNotBeBuiltIn = 3,

    /// <summary>
    /// General status code for when some value is not found, either in the database or somewhere else.
    /// </summary>
    ValueNotFound = 4,
    PhoneNumberNotFound = 5,

    /// <summary>
    /// Status code for when the user is trying to perform an action that they must be verified to do, but are not verified.<br/>
    /// For example, when the user is trying to create a campaign, but is not verified.
    /// </summary>
    VerificationStatusError = 6,

    /// <summary>
    /// Status code for when the user is trying to perform an action that requires a verification code, but the code has expired.<br/>
    /// For example, if the user tries to verify their phone number, but the code has expired.
    /// </summary>
    VerificationCodeExpired = 7,

    /// <summary>
    /// Status code for when the user is trying to perform an action that requires a specific type of authorization, but does not have it.<br/>
    /// For example, when the user tries to update a campaign, but they are not part of that campaign.
    /// </summary>
    AuthorizationError = 8,

    /// <summary>
    /// Status code for when the user is trying to perform an action that requires a specific type of permission, but does not have it.<br/>
    /// For example, when the user tries to view a page that requires a view permission, but they do not have that permission.
    /// </summary>
    PermissionError = 9,

    /// <summary>
    /// A combination of <see cref="AuthorizationError"/> and <see cref="PermissionError"/>, for when it can be either of the 2.<br/>
    /// </summary>
    PermissionOrAuthorizationError = 10,

    /// <summary>
    /// A specific type of <see cref="ValueCanNotBeNull"/> error, for when the city name is null or empty.
    /// </summary>
    CityNameRequired = 11,

    /// <summary>
    /// A status code for when a user tries to verify their phone number, but they are already verified.
    /// </summary>
    AlreadyVerified = 12,

    /// <summary>
    /// A status code for when a user tries to perform an action that requires them to be logged in, but they are not.
    /// This should generally never happen, as the [Authorize] attribute should prevent it.
    /// </summary>
    NotLoggedIn = 13,

    /// <summary>
    /// A general status code for when the user tries to perform an action with an illegal value.<br/>
    /// For example, giving a string that is too long, or a number that is below 0 or above the maximum allowed.
    /// </summary>
    IllegalValue = 14,

    /// <summary>
    /// A general status code for when a value is null or empty, but should not be.
    /// </summary>
    ValueNullOrEmpty = 15,
    
    /// <summary>
    /// A status code for when 2 values do not match, but should.<br/>
    /// For example, when the user tries to verify their phone number, but the code they entered does not match the one sent to them.
    /// </summary>
    NoMatch = 16,

    // 50000 and above are SQL errors, meant to match requirement of throwing between 50000 and 2147483647
    // These are for errors that would have been thrown by the database if not caught by the stored procedure
    // and returned as a custom error code

    /// <summary>
    /// A status code for any time when the user tries to perform an action that would result in a duplicate primary key
    /// error in the database.<br/>
    /// For example, when a user tries to assign the same user to the same job twice.
    /// </summary>
    DuplicateKey = 50001,
    ForeignKeyViolation = 50002,
    CannotInsertNull = 50003,

    /// <summary>
    /// A status code for when the user tries to add more entries than the maximum allowed.<br/>
    /// For example, when the user tries to add more than 50 roles to a campaign.
    /// </summary>
    TooManyEntries = 50004,

    /// <summary>
    /// A status code for when the user tries to insert a duplicate value into a unique index.<br/>
    /// Very similar to <see cref="DuplicateKey"/>, but for unique indexes instead of primary keys.
    /// </summary>
    CannotInsertDuplicateUniqueIndex = 50005,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a role that does not exist - a case of bad input.<br/>
    /// </summary>
    RoleNotFound = 50006,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a user that does not exist - a case of bad input.<br/>
    /// </summary>
    UserNotFound = 50007,

    /// <summary>
    /// A specific case of <see cref="CannotInsertDuplicateUniqueIndex"/>, but unlike that one, roleName is not a unique index
    /// globally in the database - it is only unique within a campaign.<br/>
    /// </summary>
    RoleAlreadyExists = 50008,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a permission that does not exist - a case of bad input.<br/>
    /// Example: when the user tries to add a permission that does not exist to a user.
    /// </summary>
    PermissionDoesNotExist = 50009,

    /// <summary>
    /// A specific case of <see cref="DuplicateKey"/> - when the user tries to add a permission to a user, but the user already has that permission.
    /// </summary>
    UserAlreadyHasPermission = 50010,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a city that does not exist - a case of bad input.<br/>
    /// </summary>
    CityNotFound = 50011,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a job that does not exist - a case of bad input.<br/>
    /// </summary>
    JobNotFound = 50012,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a job type that does not exist - a case of bad input.<br/>
    /// </summary>
    JobTypeNotFound = 50013,

    /// <summary>
    /// A status code for when a user tries to add a user to a job, but the job is already full.
    /// </summary>
    JobFullyManned = 50014,
    UserNotInCampaign = 50015,

    /// <summary>
    /// A status code when a user tries to perform an action that requires certain parameters, but they are null or empty.<br/>
    /// </summary>
    ParameterMustNotBeNullOrEmpty = 50016,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a campaign that does not exist - a case of bad input.<br/>
    /// </summary>
    CampaignNotFound = 50017,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires an event that does not exist - a case of bad input.<br/>
    /// </summary>
    EventNotFound = 50018,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a specific number of values, but they provide more than that.<br/>
    /// For example: when the user searches for a user, but provides more than 1 value to search by (such as both by email and user id).
    /// </summary>
    TooManyValuesProvided = 50019,

    /// <summary>
    /// A status code for when the user tries to assign a user to an event, but the event is already full.
    /// </summary>
    EventAlreadyFull = 50020,

    /// <summary>
    /// A status code for when the action the user is trying to perform requires a specific event type, but the event is not of that type.<br/>
    /// For example, trying to publish a private event, as only campaign events can be published.
    /// </summary>
    IncorrectEventType = 50021,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires an announcement that does not exist - a case of bad input.<br/>
    /// </summary>
    AnnouncementNotFound = 50022,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a preference that does not exist - a case of bad input.<br/>
    /// </summary>
    PreferenceNotFound = 50023,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires a financial type that does not exist - a case of bad input.<br/>
    /// </summary>
    FinancialTypeNotFound = 50024,

    /// <summary>
    /// A status code for when the user attempts to perform an action that requires a value that is not allowed by the database.<br/>
    /// For example, trying to delete a financial type that is built in, and has a big "DO NOT DELETE" description to it.
    /// </summary>
    SqlIllegalValue = 50025,

    /// <summary>
    /// A status code for when the user tries to perform an action that requires financial data that does not exist - a case of bad input.<br/>
    /// </summary>
    FinancialDataNotFound = 50026,
    
    /// <summary>
    /// A status code for when the user attempts to access a non-existent ledger.
    /// </summary>
    LedgerNotFound = 50027,
}