using DAL.Services.Interfaces;

namespace API.Utils;

public static class EventsUtils
{
    public static async Task<bool> IsEventInCampaign(IEventsService eventsService, Guid eventGuid, Guid campaignGuid)
    {
        var requestedEvent = await eventsService.GetEvent(eventGuid);
        if (requestedEvent == null)
        {
            return false;
        }
        return requestedEvent.CampaignGuid == campaignGuid;
    }
}