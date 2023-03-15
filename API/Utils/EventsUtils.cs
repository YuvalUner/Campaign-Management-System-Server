using DAL.Services.Interfaces;

namespace API.Utils;

/// <summary>
/// Was meant to be a collection of utility methods for the events service.<br/>
/// However, turned out to only contain one method,
/// <see cref="IsEventInCampaign"/>, which is used to check if an event is in a campaign.<br/>
/// </summary>
public static class EventsUtils
{
    /// <summary>
    /// Checks if an event is in a campaign.<br/>
    /// </summary>
    /// <param name="eventsService">An implementation of <see cref="IEventsService"/>, that can get the event from the database.</param>
    /// <param name="eventGuid">The Guid of the event to get.</param>
    /// <param name="campaignGuid">The Guid of the campaign that was received as user input.</param>
    /// <returns>True if it can be verified that the event belongs to the given campaign, false otherwise.</returns>
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