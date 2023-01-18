namespace DAL.Models;

public static class PermissionTypes
{
    public static readonly string View = "view";
    public static readonly string Edit = "edit";

    public static readonly string[] All = new string[] { View, Edit };

    public static bool IsValid(string permissionType)
    {
        return All.Contains(permissionType);
    }
}

public static class Screens
{
    public static readonly string CampaignSettings = "Campaign Settings";
    public static readonly string Permissions = "Permissions";

    public static readonly string[] All = new string[] { CampaignSettings, Permissions };

    public static bool IsValid(string screen)
    {
        return All.Contains(screen);
    }
}

public class Permission : IEquatable<Permission>
{
    public int? PermissionId { get; set; }

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

    private string? _screen;

    public string? PermissionForScreen
    {
        get { return _screen; }
        set
        {
            if (!Screens.IsValid(value))
            {
                throw new ArgumentException("Invalid screen");
            }

            _screen = value;
        }
    }

    public bool Equals(Permission? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _permissionType == other._permissionType && PermissionForScreen == other.PermissionForScreen;
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
        return HashCode.Combine(_permissionType, PermissionForScreen);
    }
}