namespace DAL.Models;

/// <summary>
/// A model for the jobs table.<br/>
/// </summary>
public class Job
{
    /// <summary>
    /// A unique identifier for the job.<br/>
    /// Can and should be exposed to the user.<br/>
    /// </summary>
    public Guid? JobGuid { get; set; } 
    public string? JobName { get; set; } 
    public string? JobDescription { get; set; } 
    public string? JobLocation { get; set; } 
    public DateTime? JobStartTime { get; set; } 
    public DateTime? JobEndTime { get; set; } 
    public int? JobDefaultSalary { get; set; } 
    
    public int? PeopleNeeded { get; set; }
    
    public int? PeopleAssigned { get; set; }
    
    /// <summary>
    /// Name of the job type the job belongs to.<br/>
    /// An extension that is not part of the table, but is often needed.<br/>
    /// </summary>
    public string? JobTypeName { get; set; }
}