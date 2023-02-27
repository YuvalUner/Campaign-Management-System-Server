using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IScheduleManagersService
{
    Task<IEnumerable<User>> GetScheduleManagers(string userEmail);
    Task<CustomStatusCode> AddScheduleManager(int giverUserId, string receiverEmail);
    Task<CustomStatusCode> RemoveScheduleManager(int giverUserId, string receiverEmail);
}