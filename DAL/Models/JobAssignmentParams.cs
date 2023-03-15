namespace DAL.Models;

/// <summary>
/// A model for the parameters needed when assigning a user to a job.<br/>
/// </summary>
public class JobAssignmentParams
{
    public string? UserEmail { get; set; }
    public int? Salary { get; set; }
}