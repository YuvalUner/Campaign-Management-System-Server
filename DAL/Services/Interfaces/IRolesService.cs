using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IRolesService
{
    /// <summary>
    /// Gets the role of a user in a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Role?> GetRoleInCampaign(Guid? campaignGuid, int? userId);
    
    /// <summary>
    /// Adds a custom role to a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="roleName"></param>
    /// <param name="roleDescription"></param>
    /// <returns></returns>
    Task<StatusCodes> AddRoleToCampaign(Guid? campaignGuid, string? roleName, string? roleDescription);

    /// <summary>
    /// Gets the list of roles in a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<Role>> GetRolesInCampaign(Guid? campaignGuid);

    /// <summary>
    /// Deletes a role from a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task DeleteRole(Guid? campaignGuid, string? roleName);

    /// <summary>
    /// Assigns a user to a normal role in a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="userEmail"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<StatusCodes> AssignUserToNormalRole(Guid? campaignGuid, string? userEmail, string? roleName);

    /// <summary>
    /// Assigns a user to an administrative role in a campaign and gives them all permissions.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="userEmail"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<StatusCodes> AssignUserToAdministrativeRole(Guid? campaignGuid, string? userEmail, string? roleName);

    /// <summary>
    /// Updates a role's description.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="roleName"></param>
    /// <param name="roleDescription"></param>
    /// <returns></returns>
    Task UpdateRole(Guid? campaignGuid, string? roleName, string? roleDescription);
    
    /// <summary>
    /// Removes a user from their role and sets them back to the volunteer role.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="userEmail"></param>
    /// <returns></returns>
    Task RemoveUserFromRole(Guid? campaignGuid, string? userEmail);
    
    /// <summary>
    /// Gets all the info about a role from the database, by its name and campaign.
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<Role?> GetRole(string? roleName, Guid? campaignGuid);

    /// <summary>
    /// Removes a user from their administrative role and sets them back to the volunteer role.
    /// Also removes all permissions from the user.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="userEmail"></param>
    /// <returns></returns>
    Task RemoveUserFromAdministrativeRole(Guid? campaignGuid, string? userEmail);
}