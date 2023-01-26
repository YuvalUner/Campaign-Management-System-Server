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
    
    Task<int> AddRoleToCampaign(Guid? campaignGuid, string? roleName, string? roleDescription);

    Task<IEnumerable<Role>> GetRolesInCampaign(Guid? campaignGuid);
}