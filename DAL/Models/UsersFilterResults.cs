namespace DAL.Models;

using DbAccess;

/// <summary>
/// A model for the results of a single row from the <see cref="StoredProcedureNames.FilterUsersList"/> stored procedure.<br/>
/// Contains all the information needed to be displayed about each user.
/// </summary>
public class UsersFilterResults
{
    public string? Email { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? UserPreferencesText { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ResidenceName { get; set; }
    public string? StreetName { get; set; }
    public string? HouseNumber { get; set; }
    public string? ProfilePicUrl { get; set; }
    public int? IdNum { get; set; }
}