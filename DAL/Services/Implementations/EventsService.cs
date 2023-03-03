using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;


public class EventsService: IEventsService
{
    private readonly IGenericDbAccess  _dbAccess;
    
    public EventsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<(CustomStatusCode, int?, Guid?)> AddEvent(CustomEvent customEvent)
    {
        var param = new DynamicParameters(new
        {
            customEvent.EventName,
            userId = customEvent.EventCreatorId
        });
        
        if (customEvent.EventDescription != null)
        {
            param.Add("EventDescription", customEvent.EventDescription);
        }
        if (customEvent.EventLocation != null)
        {
            param.Add("EventLocation", customEvent.EventLocation);
        }
        if (customEvent.EventStartTime != null)
        {
            param.Add("EventStartTime", customEvent.EventStartTime);
        }
        if (customEvent.EventEndTime != null)
        {
            param.Add("EventEndTime", customEvent.EventEndTime);
        }
        if (customEvent.CampaignGuid != null)
        {
            param.Add("CampaignGuid", customEvent.CampaignGuid);
        }
        if (customEvent.MaxAttendees != null)
        {
            param.Add("MaxAttendees", customEvent.MaxAttendees);
        }
        if (customEvent.IsOpenJoin != null)
        {
            param.Add("IsOpenJoin", customEvent.IsOpenJoin);
        }
        if (customEvent.EventOf != null)
        {
            param.Add("EventOf", customEvent.EventOf);
        }
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        param.Add("newEventGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        param.Add("newEventId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AddEvent, param);
        return (param.Get<CustomStatusCode>("returnVal"), param.Get<int>("newEventId"), param.Get<Guid>("newEventGuid"));
    }
    
    public async Task<CustomStatusCode> UpdateEvent(CustomEvent customEvent)
    {
        var param = new DynamicParameters(new
        {
            customEvent.EventGuid
        });
        
        if (customEvent.EventDescription != null)
        {
            param.Add("EventDescription", customEvent.EventDescription);
        }
        if (customEvent.EventLocation != null)
        {
            param.Add("EventLocation", customEvent.EventLocation);
        }
        if (customEvent.EventStartTime != null)
        {
            param.Add("EventStartTime", customEvent.EventStartTime);
        }
        if (customEvent.EventEndTime != null)
        {
            param.Add("EventEndTime", customEvent.EventEndTime);
        }
        if (customEvent.CampaignGuid != null)
        {
            param.Add("CampaignGuid", customEvent.CampaignGuid);
        }
        if (customEvent.MaxAttendees != null)
        {
            param.Add("MaxAttendees", customEvent.MaxAttendees);
        }
        if (customEvent.EventName != null)
        {
            param.Add("EventName", customEvent.EventName);
        }
        if (customEvent.IsOpenJoin != null)
        {
            param.Add("IsOpenJoin", customEvent.IsOpenJoin);
        }
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.UpdateEvent, param);
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> DeleteEvent(Guid eventGuid)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.DeleteEvent, param);
        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> AddEventParticipant(Guid eventGuid, int? userId = null, string? userEmail = null)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        if (userId != null)
        {
            param.Add("userId", userId);
        }
        
        if (userEmail != null)
        {
            param.Add("userEmail", userEmail);
        }
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AssignToEvent, param);
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<IEnumerable<GetUserEventsResult>> GetUserEvents(int userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        var result = await _dbAccess.GetData<GetUserEventsResult, DynamicParameters>(StoredProcedureNames.GetUserEvents, param);
        return result;
    }

    public async Task<CustomStatusCode> AddEventWatcher(int userId, Guid eventGuid)
    {
        var param = new DynamicParameters(new
        {
            userId,
            eventGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddWatcherToEvent, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> RemoveEventWatcher(int userId, Guid eventGuid)
    {
        var param = new DynamicParameters(new
        {
            userId,
            eventGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveEventWatcher, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> RemoveEventParticipant(Guid eventGuid, int? userId = null, string? userEmail = null)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        if (userId != null)
        {
            param.Add("userId", userId);
        }
        
        if (userEmail != null)
        {
            param.Add("userEmail", userEmail);
        }
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveEventParticipant, param);
        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<IEnumerable<EventWithCreatorDetails?>> GetCampaignEvents(Guid? campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });

        var res =  await _dbAccess.GetData<EventWithCreatorDetails,
            DynamicParameters>(StoredProcedureNames.GetCampaignEvents, param);
        
        return  res;
    }
    
    public async Task<(CustomStatusCode, IEnumerable<UserPublicInfo>)> GetEventParticipants(Guid eventGuid)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        var res =  await _dbAccess.GetData<UserPublicInfo,
            DynamicParameters>(StoredProcedureNames.GetEventParticipants, param);
        
        return (param.Get<CustomStatusCode>("returnVal") ,res);
    }
    
    public async Task<EventWithCreatorDetails?> GetEvent(Guid eventGuid)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        
        var res =  await _dbAccess.GetData<EventWithCreatorDetails,
            DynamicParameters>(StoredProcedureNames.GetEvent, param);
        
        return res.FirstOrDefault();
    }

    public async Task<User?> GetEventCreatorUserId(Guid eventGuid)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        var res =  await _dbAccess.GetData<User,
            DynamicParameters>(StoredProcedureNames.GetEventCreatorUserId, param);
        
        return res.FirstOrDefault();
    }

    public async Task<IEnumerable<EventWithCreatorDetails>> GetPersonalEvents(int userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        
        return await _dbAccess.GetData<EventWithCreatorDetails, DynamicParameters>(StoredProcedureNames.GetPersonalEvents, param);
    }
    
    public async Task<(CustomStatusCode,IEnumerable<UserPublicInfo>)> GetEventWatchers(Guid eventGuid)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        var res =  await _dbAccess.GetData<UserPublicInfo, DynamicParameters>(StoredProcedureNames.GetEventWatchers, param);
        
        return (param.Get<CustomStatusCode>("returnVal"), res);
    }
}