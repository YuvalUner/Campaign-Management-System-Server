namespace DAL.Models;

public class UserJob
{
    public int? Salary { get; set; } 
    public string? JobTypeName { get; set; } 
    public string? JobDescription { get; set; } 
    public string? JobName { get; set; } 
    public string? JobLocation { get; set; } 
    public DateTime? JobStartTime { get; set; } 
    public DateTime? JobEndTime { get; set; } 
    public string? Email { get; set; } 
    public string? FirstNameHeb { get; set; } 
    public string? LastNameHeb { get; set; } 
    public string? DisplayNameEng { get; set; } 
    public string? ProfilePicUrl { get; set; } 
    public string? PhoneNumber { get; set; } 
    public string? CampaignName { get; set; } 
    public Guid? CampaignGuid { get; set; }
    public Guid? JobGuid { get; set; }
}