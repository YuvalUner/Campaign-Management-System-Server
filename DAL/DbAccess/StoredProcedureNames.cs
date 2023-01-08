namespace DAL.DbAccess;

public static class StoredProcedureNames
{
    /// <summary>
    /// Tests the connection to the database by executing a stored procedure that returns a single value.
    /// </summary>
    public static readonly string TestConnection = "usp_TestConnection";

    
    /// <summary>
    /// Gets a user's info by their email.
    /// Params: email (string)
    /// </summary>
    public static readonly string GetAllUserInfoByEmail = "usp_GetAllUserInfoByEmail";

    /// <summary>
    /// Adds a user to the users table.
    /// Params: email (String), firstNameEng (string), lastNameEng (string), displayNameEng(str), profilePicUrl (str)
    /// </summary>
    public static readonly string CreateUser = "usp_InsertUser";
}