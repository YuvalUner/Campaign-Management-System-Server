namespace DAL.Models;

/// <summary>
/// A static class containing the names of the built-in roles.<br/>
/// Used to check if a role is a built-in role, and act as needed in case it is.<br/>
/// </summary>
public static class BuiltInRoleNames
{
    public static readonly string Owner = "Campaign Owner";
    public static readonly string Manager = "Campaign Manager";
    public static readonly string Candidate = "Candidate";
    public static readonly string Volunteer = "Volunteer";
    public static readonly string Worker = "Worker";

    private static readonly string[] All = new[] { Owner, Manager, Candidate, Volunteer, Worker };

    /// <summary>
    /// Checks if the given role name is a built-in role.<br/>
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns>True if the role is a built in one, false otherwise.</returns>
    public static bool IsBuiltInRole(string roleName)
    {
        return All.Contains(roleName);
    }
}

/// <summary>
/// A model for the roles table of the database.<br/>
/// </summary>
public class Role
{
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? RoleDescription { get; set; }

    /// <summary>
    /// Id of the campaign the role belongs to.
    /// </summary>
    public int? CampaignId { get; set; }

    /// <summary>
    /// If this value is greater than 0, the role is an admin role.<br/>
    /// </summary>
    public int RoleLevel { get; set; }

    public bool IsCustomRole { get; set; }
}