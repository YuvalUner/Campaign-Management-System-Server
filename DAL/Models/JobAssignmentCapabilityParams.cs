namespace DAL.Models;

/// <summary>
/// A model for the parameters needed when assigning a user who can assign to a job.<br/>
/// </summary>
public class JobAssignmentCapabilityParams
{
    public Guid JobGuid { get; set; }
    public string? UserEmail { get; set; }
}