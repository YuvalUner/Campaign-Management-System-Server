using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for adding, removing, and getting permissions for a user in a campaign.
/// </summary>
public interface IPermissionsService
{
    /// <summary>
    /// Adds a permission to a user in a campaign
    /// </summary>
    /// <param name="permission">An instance of <see cref="Permission"/> with target and type not null.</param>
    /// <param name="userId">User id of the user to add a permission for.</param>
    /// <param name="campaignGuid">Guid of the campaign to add a permission in.</param>
    /// <returns>Status Code <see cref="CustomStatusCode.PermissionDoesNotExist"/> if the permission does not exist,
    /// <see cref="CustomStatusCode.UserAlreadyHasPermission"/> if the user already has the permission.</returns>
    Task<CustomStatusCode> AddPermission(Permission permission, int? userId, Guid? campaignGuid);

    /// <summary>
    /// Gets all permissions for a user in a campaign
    /// </summary>
    /// <param name="userId">User id of the user to get permissions for.</param>
    /// <param name="campaignGuid">Guid of the campaign to get the permission in.</param>
    /// <returns>A list of <see cref="Permission"/> for this specific user in the specific campaign.</returns>
    Task<IEnumerable<Permission?>> GetPermissions(int? userId, Guid? campaignGuid);

    /// <summary>
    /// Removes a permission from a user in a campaign
    /// </summary>
    /// <param name="permission">An instance of <see cref="Permission"/> with target and type filled in.</param>
    /// <param name="userId">User id of the user to remove a permission from.</param>
    /// <param name="campaignGuid">Guid of the campaign to remove a permission in.</param>
    /// <returns></returns>
    Task RemovePermission(Permission permission, int? userId, Guid? campaignGuid);
}