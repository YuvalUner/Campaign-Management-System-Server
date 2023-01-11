namespace DAL.Models;

/// <summary>
/// A model for representing the data needed when loading the home page.
/// Meant to contain the rows from the output of the stored procedure GetUserCampaigns.
/// </summary>
public class CampaignUser
{
    public int? UserId { get; set; }
    public int? CampaignId { get; set; }
    public string? CampaignName { get; set; }
    public Guid? CampaignGuid { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
}