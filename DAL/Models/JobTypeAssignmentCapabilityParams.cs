namespace DAL.Models;

/// <summary>
/// A model for the parameters needed when assigning a user who can assign to any job of a certain job type.<br/>
/// </summary>
public class JobTypeAssignmentCapabilityParams
{
    public string? JobTypeName { get; set; }
    public string? UserEmail { get; set; }
}