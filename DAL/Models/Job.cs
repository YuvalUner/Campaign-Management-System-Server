namespace DAL.Models;

public class Job
{
    public int? JobId { get; set; } 
    public int? CampaignId { get; set; } 
    public Guid? JobGuid { get; set; } 
    public string? JobName { get; set; } 
    public string? JobDescription { get; set; } 
    public string? JobLocation { get; set; } 
    public DateTime? JobStartTime { get; set; } 
    public DateTime? JobEndTime { get; set; } 
    public int? JobDefaultSalary { get; set; } 
    
    public int? PeopleNeeded { get; set; }
    
    public int? PeopleAssigned { get; set; }
    
    public string? JobTypeName { get; set; }
}