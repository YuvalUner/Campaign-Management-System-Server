namespace DAL.Models;

/// <summary>
/// The parameters for filtering users for jobs.<br/>
/// All parameters but CampaignGuid are optional, and can be used to filter the users in any way.
/// </summary>
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

    /// <summary>
    /// When the job starts. Used to filter users who are not available at that time.
    /// </summary>
    public DateTime? JobStartTime { get; set; }

    /// <summary>
    /// When the job ends. Used to filter users who are not available at that time.
    /// </summary>
    public DateTime? JobEndTime { get; set; }
}