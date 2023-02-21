using System.Data;
using DAL.DbAccess;
using DAL.Models;
using Dapper;

namespace DAL.Services.Implementations;

public class EventsService
{
    private readonly IGenericDbAccess  _dbAccess;
    
    public EventsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<(CustomStatusCode, int, Guid)> AddEvent(CustomEvent customEvent)
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
            param.Add("CampaignGuid", customEvent.CampaignId);
        }
        if (customEvent.MaxAttendees != null)
        {
            param.Add("MaxAttendees", customEvent.MaxAttendees);
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
            param.Add("CampaignGuid", customEvent.CampaignId);
        }
        if (customEvent.MaxAttendees != null)
        {
            param.Add("MaxAttendees", customEvent.MaxAttendees);
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

    public async Task<CustomStatusCode> AssignToEvent(Guid eventGuid, int? userId, string? userEmail)
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
}