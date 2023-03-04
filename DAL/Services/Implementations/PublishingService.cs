using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class PublishingService : IPublishingService
{
    private readonly IGenericDbAccess _dbAccess;

    public PublishingService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<CustomStatusCode> PublishEvent(Guid? eventGuid, int? publisherId)
    {
        var param = new DynamicParameters(new
        {
            eventGuid,
            publisherId
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.PublishEvent, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> UnpublishEvent(Guid? eventGuid)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.UnpublishEvent, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<(CustomStatusCode, IEnumerable<PublishedEventWithPublisher>)> GetCampaignPublishedEvents(
        Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        var result =
            await _dbAccess.GetData<PublishedEventWithPublisher, DynamicParameters>(
                StoredProcedureNames.GetCampaignPublishedEvents, param);

        return (param.Get<CustomStatusCode>("returnVal"), result);
    }

    public async Task<(CustomStatusCode, Guid)> PublishAnnouncement(Announcement announcement, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            announcement.AnnouncementTitle,
            announcement.AnnouncementContent,
            announcement.PublisherId,
            campaignGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        param.Add("newAnnouncementGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);

        await _dbAccess.ModifyData(StoredProcedureNames.PublishAnnouncement, param);

        return (param.Get<CustomStatusCode>("returnVal"), param.Get<Guid>("newAnnouncementGuid"));
    }
    
    public async Task<CustomStatusCode> UnpublishAnnouncement(Guid? announcementGuid)
    {
        var param = new DynamicParameters(new
        {
            announcementGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.UnpublishAnnouncement, param);

        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<(CustomStatusCode, IEnumerable<AnnouncementWithPublisherDetails>)> GetCampaignAnnouncements(Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        var result =
            await _dbAccess.GetData<AnnouncementWithPublisherDetails, DynamicParameters>(
                StoredProcedureNames.GetCampaignPublishedAnnouncements, param);

        return (param.Get<CustomStatusCode>("returnVal"), result);
    }
}