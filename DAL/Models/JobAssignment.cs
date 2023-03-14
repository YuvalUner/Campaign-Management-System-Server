namespace DAL.Models;
using DbAccess;

/// <summary>
/// A model for a single returning row from <see cref="StoredProcedureNames.GetUsersAssignedToJob"/> stored procedure.<br/>
/// Each row is for a single user assigned to a job.<br/>
/// </summary>
public class JobAssignment
{
    public string? DisplayNameEng { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? Email { get; set; }
    public int? Salary { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePicUrl { get; set; }
}