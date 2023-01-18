using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IPermissionsService
{
    Task AddPermission(Permission permission, int? userId, Guid? campaignGuid);
}