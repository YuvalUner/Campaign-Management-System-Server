using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IPermissionsService
{
    /// <summary>
    /// Adds a permission to a user in a campaign
    /// </summary>
    /// <param name="permission"></param>
    /// <param name="userId"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task AddPermission(Permission permission, int? userId, Guid? campaignGuid);

    /// <summary>
    /// Gets all permissions for a user in a campaign
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<Permission?>> GetPermissions(int? userId, Guid? campaignGuid);
}