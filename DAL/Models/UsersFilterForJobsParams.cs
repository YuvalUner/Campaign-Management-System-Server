namespace DAL.Models;

public class UsersFilterForJobsParams
{
    public Guid? CampaignGuid { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? CityName { get; set; }
    public string? StreetName { get; set; }
    public int? IdNum { get; set; }
    public DateTime? JobStartTime { get; set; }
    public DateTime? JobEndTime { get; set; }
}