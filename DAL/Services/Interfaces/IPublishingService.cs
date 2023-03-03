using DAL.DbAccess;

namespace DAL.Services.Interfaces;

public interface IPublishingService
{
    Task<CustomStatusCode> PublishEvent(Guid? eventGuid, int? publisherId);
    Task<CustomStatusCode> UnpublishEvent(Guid? eventGuid);
}