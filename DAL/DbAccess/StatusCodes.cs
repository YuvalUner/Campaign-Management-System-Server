namespace DAL.DbAccess;

public enum StatusCodes
{
    // 0 is the default return value for stored procedures, so we use it for success
    Ok = 0,
    IdAlreadyExistsWhenVerifyingInfo = 1,
    
    // 50000 and above are SQL errors, meant to match requirement of throwing between 50000 and 2147483647
    DuplicateKey = 50001,
    ForeignKeyViolation = 50002,
    CannotInsertNull = 50003,
    TooManyEntries = 50004,
    CannotInsertDuplicateUniqueIndex = 50005,
    RoleNotFound = 50006,
    UserNotFound = 50007,
    RoleAlreadyExists = 50008,
}