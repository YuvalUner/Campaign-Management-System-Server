using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IRolesService
{
    Task<Role?> GetRoleInCampaign(Guid? campaignGuid, int? userId);
}