using System.Data;
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

    public async Task<IEnumerable<PublishedEventWithPublisher>> GetEventsForUser(int? userId, int? limit, int? offset)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        if (limit.HasValue)
        {
            param.Add("limit", limit);
        }
        if (offset.HasValue)
        {
            param.Add("offset", offset);
        }

        return await _dbAccess.GetData<PublishedEventWithPublisher, DynamicParameters>
            (StoredProcedureNames.GetPublishedEventsByUserPreferences, param);
    }

    public async Task<IEnumerable<AnnouncementWithPublisherDetails>> GetAnnouncementsForUser(int? userId, int? limit, int? offset)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        if (limit.HasValue)
        {
            param.Add("limit", limit);
        }
        if (offset.HasValue)
        {
            param.Add("offset", offset);
        }
        
        return await _dbAccess.GetData<AnnouncementWithPublisherDetails, DynamicParameters>
            (StoredProcedureNames.GetPublishedAnnouncementsByUserPreferences, param);
    }

    public async Task<IEnumerable<PublishedEventWithPublisher>> SearchEvents(EventsSearchParams searchParams)
    {
        return await _dbAccess.GetData<PublishedEventWithPublisher, EventsSearchParams>
            (StoredProcedureNames.SearchPublishedEvents, searchParams);
    }
    
    public async Task<IEnumerable<AnnouncementWithPublisherDetails>> SearchAnnouncements(AnnouncementsSearchParams searchParams)
    {
        return await _dbAccess.GetData<AnnouncementWithPublisherDetails, AnnouncementsSearchParams>
            (StoredProcedureNames.SearchPublishedAnnouncements, searchParams);
    }

    public async Task<CustomStatusCode> AddNotificationSettings(NotificationUponPublishSettings settings)
    {
        var param = new DynamicParameters(new
        {
            settings.UserId,
            settings.CampaignGuid,
            settings.ViaSms,
            settings.ViaEmail
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.ModifyUserNotificationSettingsOnPublish, param);
        
        return  param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> RemoveNotificationSettings(NotificationUponPublishSettings settings)
    {
        // A wrapper method for AddNotificationSettings, which is used to update the settings.
        // This is due to the stored procedure being used to add, update or delete the settings.
        // Made purely for the sake of clarity.
        settings.ViaEmail = false;
        settings.ViaSms = false;
        
        return await AddNotificationSettings(settings);
    }

    
    public async Task<CustomStatusCode> UpdateNotificationSettings(NotificationUponPublishSettings settings)
    {
        // A wrapper method for AddNotificationSettings, which is used to update the settings.
        // This is due to the stored procedure being used to add, update or delete the settings.
        // Made purely for the sake of clarity.
        return await AddNotificationSettings(settings);
    }
    
    public async Task<IEnumerable<NotificationUponPublishSettingsForUser>> GetNotificationSettingsForUser(int userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        
        return await _dbAccess.GetData<NotificationUponPublishSettingsForUser, DynamicParameters>
            (StoredProcedureNames.GetUserNotificationSettingsOnPublishForUser, param);
    }
    
    public async Task<IEnumerable<NotificationUponPublishSettingsForCampaign>> GetNotificationSettingsForCampaign(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        
        return await _dbAccess.GetData<NotificationUponPublishSettingsForCampaign, DynamicParameters>
            (StoredProcedureNames.GetUserNotificationSettingsOnPublishForCampaign, param);
    }
}