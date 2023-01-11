namespace DAL.Models;

public class Role
{
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? RoleDescription { get; set; }
    public int? CampaignId { get; set; }
}