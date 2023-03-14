namespace DAL.Models;

/// <summary>
/// An extension of <see cref="Job"/>.<br/>
/// Contains information about the job, as well as user specific information for it and also information
/// about the campaign the job belongs to.<br/>
/// </summary>
public class UserJob : Job
{
    public int? Salary { get; set; }
    public string? Email { get; set; }
    public string? FirstNameHeb { get; set; }
    public string? LastNameHeb { get; set; }
    public string? DisplayNameEng { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CampaignName { get; set; }
    public Guid? CampaignGuid { get; set; }
}