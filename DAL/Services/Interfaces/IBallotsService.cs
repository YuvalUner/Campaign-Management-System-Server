using DAL.DbAccess;
using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IBallotsService
{
    /// <summary>
    /// Adds a new custom ballot for a campaign.
    /// </summary>
    /// <param name="ballot"></param>
    /// <param name="campaignGuid"></param>
    /// <returns>CampaignNotFound if campaignGuid is that of a non-existent campaign, DuplicateKey
    /// if the user tries to add a ballot with an already existing id.</returns>
    Task<CustomStatusCode> AddCustomBallot(Ballot ballot, Guid campaignGuid);
    
    /// <summary>
    /// Updates an existing custom ballot for a campaign.
    /// </summary>
    /// <param name="ballot"></param>
    /// <param name="campaignGuid"></param>
    /// <returns>CampaignNotFound if campaignGuid is that of a non-existent campaign.</returns>
    Task<CustomStatusCode> UpdateCustomBallot(Ballot ballot, Guid campaignGuid);
    
    /// <summary>
    /// Deletes an existing custom ballot for a campaign.
    /// </summary>
    /// <param name="innerCityBallotId"></param>
    /// <param name="campaignGuid"></param>
    /// <returns>CampaignNotFound if campaignGuid is that of a non-existent campaign. </returns>
    Task<CustomStatusCode> DeleteCustomBallot(Decimal innerCityBallotId, Guid campaignGuid);
    
    /// <summary>
    /// Gets all the ballots for a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<Ballot>> GetAllBallotsForCampaign(Guid campaignGuid);
}