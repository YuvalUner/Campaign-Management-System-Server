namespace DAL.Models;

using DbAccess;

/// <summary>
/// A model for representing the data needed when loading the home page, showing each campaign the user belongs to
/// and their role in it.<br/>
/// Meant to contain the rows from the output of the stored procedure <see cref="StoredProcedureNames.GetUserCampaigns"/>.
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