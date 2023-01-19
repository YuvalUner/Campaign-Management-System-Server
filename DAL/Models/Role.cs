namespace DAL.Models;

public static class BuiltInRoleNames
{
    public static readonly string Owner = "Campaign Owner";
    public static readonly string Manager = "Campaign Manager";
    public static readonly string Candidate = "Candidate";
    public static readonly string Volunteer = "Volunteer";
    public static readonly string Worker = "Worker";
    
    private static readonly string[] All = new[] { Owner, Manager, Candidate, Volunteer, Worker };
    
    public static bool IsBuiltInRole(string roleName)
    {
        return All.Contains(roleName);
    }
}
public class Role
{
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? RoleDescription { get; set; }
    public int? CampaignId { get; set; }
    public int RoleLevel { get; set; }
}