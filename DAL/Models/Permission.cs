namespace DAL.Models;

/// <summary>
/// A static collection of all the possible permission types.<br/>
/// Only these types are allowed for use.<br/>
/// </summary>
public static class PermissionTypes
{
    public const string View = "view";
    public const string Edit = "edit";

    private static readonly string[] All = { View, Edit };

    /// <summary>
    /// Checks if the given permission type is valid.<br/>
    /// </summary>
    /// <param name="permissionType">The name of the permission type to check.</param>
    /// <returns>True if the given permissionType is one the built in ones, false otherwise.</returns>
    public static bool IsValid(string permissionType)
    {
        return All.Contains(permissionType);
    }
}

/// <summary>
/// A collection of all the possible permission targets.<br/>
/// Only these targets are allowed for use.<br/>
/// Permission targets are the screens that the user can view or edit.<br/>
/// </summary>
public static class PermissionTargets
{
    public const string CampaignSettings = "Campaign Settings";
    public const string Permissions = "Permissions";
    public const string VotersLedger = "Voters Ledger";
    public const string CampaignUsersList = "Campaign Users List";
    public const string CampaignRolesList = "Campaign Roles List";
    public const string CampaignRoles = "Campaign Roles";
    public const string Jobs = "Jobs";
    public const string JobTypes = "Job Types";
    public const string Sms = "SMS";
    public const string Events = "Events";
    public const string Publishing = "Publishing";
    public const string Financial = "Financial";
    public const string CustomVotersLedger = "Custom Voters Ledger";

    private static readonly string[] All =
    {
        CampaignSettings,
        Permissions,
        VotersLedger,
        CampaignUsersList,
        CampaignRolesList,
        CampaignRoles,
        Jobs,
        JobTypes,
        Sms,
        Events,
        Publishing,
        Financial,
        CustomVotersLedger
    };

    /// <summary>
    /// Checks if the given target is valid.<br/>
    /// </summary>
    /// <param name="screen">Name of the screen that is the permission target to check for</param>
    /// <returns>True if the target is a built in one, false otherwise.</returns>
    public static bool IsValid(string screen)
    {
        return All.Contains(screen);
    }
}

/// <summary>
/// A model for the permissions table of the database.<br/>
/// Will throw an exception if the permission type or target are invalid.<br/>
/// Can be compared to other permissions.<br/>
/// </summary>
public class Permission : IEquatable<Permission>
{
    private readonly string? _permissionType;

    public string? PermissionType
    {
        get => _permissionType;
        init
        {
            if (!PermissionTypes.IsValid(value))
            {
                throw new ArgumentException($"Invalid permission type {value}");
            }

            _permissionType = value;
        }
    }

    private readonly string? _target;

    public string? PermissionTarget
    {
        get => _target;
        init
        {
            if (!PermissionTargets.IsValid(value))
            {
                throw new ArgumentException($"Invalid permission target {value}");
            }

            _target = value;
        }
    }

    public bool Equals(Permission? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _permissionType == other._permissionType && PermissionTarget == other.PermissionTarget;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Permission)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_permissionType, PermissionTarget);
    }
}