namespace DAL.Models;

/// <summary>
/// An extension of <see cref="UserPublicInfo"/>.<br/>
/// Contains additional information about the user's role in the campaign.<br/>
/// </summary>
public class UserInCampaign : UserPublicInfo
{
    public string? RoleName { get; set; }
    public string? RoleLevel { get; set; }
}