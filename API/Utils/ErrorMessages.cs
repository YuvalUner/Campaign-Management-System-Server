using StatusCodes = DAL.DbAccess.StatusCodes;

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
    
    public static string FormatErrorMessage(string message, StatusCodes statusCode)
    {
        return $"Error Num {(int)statusCode} - {message}";
    }
}