using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class PublicBoardService: IPublicBoardService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public PublicBoardService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<IEnumerable<PublishedEventWithPublisher>> GetEventsForUser(int? userId, int? limit)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        if (limit.HasValue)
        {
            param.Add("limit", limit);
        }

        return await _dbAccess.GetData<PublishedEventWithPublisher, DynamicParameters>
            (StoredProcedureNames.GetPublishedEventsByUserPreferences, param);
    }

    public async Task<IEnumerable<AnnouncementWithPublisherDetails>> GetAnnouncementsForUser(int? userId, int? limit)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        if (limit.HasValue)
        {
            param.Add("limit", limit);
        }
        
        return await _dbAccess.GetData<AnnouncementWithPublisherDetails, DynamicParameters>
            (StoredProcedureNames.GetPublishedAnnouncementsByUserPreferences, param);
    }
}