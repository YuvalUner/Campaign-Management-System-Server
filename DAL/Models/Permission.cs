namespace DAL.Models;

public static class PermissionTypes
{
    public static readonly string View = "view";
    public static readonly string Edit = "edit";

    private static readonly string[] All = new string[] { View, Edit };

    public static bool IsValid(string permissionType)
    {
        return All.Contains(permissionType);
    }
}

public static class PermissionTargets
{
    public static readonly string CampaignSettings = "Campaign Settings";
    public static readonly string Permissions = "Permissions";
    public static readonly string VotersLedger = "Voters Ledger";
    public static readonly string CampaignUsersList = "Campaign Users List";

    private static readonly string[] All = new string[] { CampaignSettings, Permissions, VotersLedger };

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
        get { return _permissionType; }
        set
        {
            if (!PermissionTypes.IsValid(value))
            {
                throw new ArgumentException("Invalid permission type");
            }

            _permissionType = value;
        }
    }

    private string? _target;

    public string? PermissionTarget
    {
        get { return _target; }
        set
        {
            if (!PermissionTargets.IsValid(value))
            {
                throw new ArgumentException("Invalid screen name");
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