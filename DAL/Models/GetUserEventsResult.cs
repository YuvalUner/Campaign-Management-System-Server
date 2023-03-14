namespace DAL.Models;
using DbAccess;

/// <summary>
/// An extension of <see cref="CustomEvent"/>, meant to represent a single returning row from the
/// <see cref="StoredProcedureNames.GetUserEvents"/> stored procedure.<br/>
/// Contains additional information about the event - campaign's name and logo, as well as if the user is a participant
/// in the event.<br/>
/// </summary>
public class GetUserEventsResult: CustomEvent
{
    public string? CampaignName { get; set; }
    public string? CampaignLogoUrl { get; set; }
    public bool Participating { get; set; }
}