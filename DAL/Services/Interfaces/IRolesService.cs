using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service performing CRUD operations on roles, as well as assigning users to roles.
/// </summary>
public interface IRolesService
{
    /// <summary>
    /// Gets the role of a user in a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userId">User id of the user to get the role for.</param>
    /// <returns>An instance of <see cref="Role"/> if the user is part of the campaign, null otherwise.</returns>
    Task<Role?> GetRoleInCampaign(Guid? campaignGuid, int? userId);
    
    /// <summary>
    /// Adds a custom role to a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to add a role for.</param>
    /// <param name="roleName">Name of the new role to add.</param>
    /// <param name="roleDescription">Description of the role to add.</param>
    /// <returns><see cref="CustomStatusCode.TooManyEntries"/> if number of roles is 50 or more,
    /// <see cref="CustomStatusCode.RoleAlreadyExists"/> if role already exists for the campaign</returns>
    Task<CustomStatusCode> AddRoleToCampaign(Guid? campaignGuid, string? roleName, string? roleDescription);

    /// <summary>
    /// Gets the list of roles in a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>A list of all <see cref="Role"/>s belonging to the campaign.</returns>
    Task<IEnumerable<Role>> GetRolesInCampaign(Guid? campaignGuid);

    /// <summary>
    /// Deletes a role from a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="roleName">Name of the campaign to delete.</param>
    /// <returns></returns>
    Task DeleteRole(Guid? campaignGuid, string? roleName);

    /// <summary>
    /// Assigns a user to a normal role in a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to assign to the role.</param>
    /// <param name="roleName">Name of the role to assign to.</param>
    /// <returns><see cref="CustomStatusCode.UserNotFound"/> if user not found,
    /// <see cref="CustomStatusCode.RoleNotFound"/> if user not found.</returns>
    Task<CustomStatusCode> AssignUserToNormalRole(Guid? campaignGuid, string? userEmail, string? roleName);

    /// <summary>
    /// Assigns a user to an administrative role in a campaign and gives them all permissions.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to assign to the role.</param>
    /// <param name="roleName">Name of the role to assign to.</param>
    /// <returns><see cref="CustomStatusCode.DuplicateKey"/> if user is already in an admin role,
    /// <see cref="CustomStatusCode.UserNotFound"/> if user not found,
    /// <see cref="CustomStatusCode.RoleNotFound"/> if role was not found.</returns>
    Task<CustomStatusCode> AssignUserToAdministrativeRole(Guid? campaignGuid, string? userEmail, string? roleName);

    /// <summary>
    /// Updates a role's description.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="roleName">Name of the role to update.</param>
    /// <param name="roleDescription">New description of the role.</param>
    /// <returns></returns>
    Task UpdateRole(Guid? campaignGuid, string? roleName, string? roleDescription);
    
    /// <summary>
    /// Removes a user from their role and sets them back to the volunteer role.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to remove from the role.</param>
    /// <returns></returns>
    Task RemoveUserFromRole(Guid? campaignGuid, string? userEmail);
    
    /// <summary>
    /// Gets all the info about a role from the database, by its name and campaign.
    /// </summary>
    /// <param name="roleName">Name of the role to get the data about.</param>
    /// <param name="campaignGuid">Guid of the campaign the role belongs to.</param>
    /// <returns></returns>
    Task<Role?> GetRole(string? roleName, Guid? campaignGuid);

    /// <summary>
    /// Removes a user from their administrative role and sets them back to the volunteer role.
    /// Also removes all permissions from the user.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userEmail">Email of the user to remove from the role.</param>
    /// <returns></returns>
    Task RemoveUserFromAdministrativeRole(Guid? campaignGuid, string? userEmail);
}