namespace DAL.Models;

using DbAccess;

/// <summary>
/// A model for a single row of the <see cref="StoredProcedureNames.GetSmsDetailsLog"/> stored procedure.<br/>
/// Contains details on each person that was sent a specific SMS.<br/>
/// </summary>
public class SmsDetailsLogResult
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    /// <summary>
    /// True if the SMS was sent successfully, false otherwise.
    /// </summary>
    public bool IsSuccess { get; set; }

    public string? ResidenceName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StreetName { get; set; }
    public string? HouseNumber { get; set; }
}