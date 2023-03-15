namespace DAL.Models;

/// <summary>
/// A model for parameters used when filtering jobs.<br/>
/// All properties are nullable, so that the user can choose which parameters to use.<br/>
/// </summary>
public class JobsFilterParameters
{
    public string? JobTypeName { get; set; }
    public DateTime? JobStartTime { get; set; }
    public DateTime? JobEndTime { get; set; }
    public bool? TimeFromStart { get; set; }
    public bool? TimeBeforeStart { get; set; }
    public bool? TimeFromEnd { get; set; }
    public bool? TimeBeforeEnd { get; set; }
    public string? JobName { get; set; }
    public bool? FullyManned { get; set; }
    public bool? OnlyCustomJobTypes { get; set; }
    public string? JobLocation { get; set; }
}