namespace DAL.Models;

public class CampaignAdminUserInfo: UserPublicInfo
{
    public string? RoleName { get; set; }
    public int? RoleLevel { get; set; }
}