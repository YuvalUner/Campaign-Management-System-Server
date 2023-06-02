using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IPartiesService
{
    /// <summary>
    /// Adds a new party to the database for the given campaign.
    /// </summary>
    /// <param name="party"></param>
    /// <param name="campaignGuid"></param>
    /// <returns>CampaignNotFound in case of bad campaignGuid.</returns>
    Task<CustomStatusCode> AddParty(Party party, Guid campaignGuid);
    
    /// <summary>
    /// Updates an existing party in the database for the given campaign.
    /// </summary>
    /// <param name="party"></param>
    /// <param name="campaignGuid"></param>
    /// <returns>CampaignNotFound in case of bad campaignGuid.</returns>
    Task<CustomStatusCode> UpdateParty(Party party, Guid campaignGuid);
    
    /// <summary>
    /// Deletes an existing party in the database for the given campaign.
    /// </summary>
    /// <param name="partyId"></param>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<CustomStatusCode> DeleteParty(int partyId, Guid campaignGuid);
    
    /// <summary>
    /// Gets a list of all parties registered for the given campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<Party>> GetParties(Guid campaignGuid);
}