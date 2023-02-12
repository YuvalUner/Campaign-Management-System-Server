namespace DAL.Models;

public static class PermissionTypes
{
    public const string View = "view";
    public const string Edit = "edit";

    private static readonly string[] All = { View, Edit };

    public static bool IsValid(string permissionType)
    {
        return All.Contains(permissionType);
    }
}

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

    private static readonly string[] All =  { 
        CampaignSettings, 
        Permissions,
        VotersLedger,
        CampaignUsersList,
        CampaignRolesList, 
        CampaignRoles,
        Jobs,
        JobTypes,
        Sms
    };

    public static bool IsValid(string screen)
    {
        return All.Contains(screen);
    }
}

public class Permission : IEquatable<Permission>
{
    private string? _permissionType;

    public string? PermissionType
    {
        get => _permissionType;
        set
        {
            if (!PermissionTypes.IsValid(value))
            {
                throw new ArgumentException($"Invalid permission type {value}");
            }

            _permissionType = value;
        }
    }

    private string? _target;

    public string? PermissionTarget
    {
        get => _target;
        set
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