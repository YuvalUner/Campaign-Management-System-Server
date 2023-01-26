using DAL.Models;

namespace DAL.Services.Interfaces;

public interface ICampaignsService
{
    /// <summary>
    /// Adds a campaign to the database.
    /// </summary>
    /// <param name="campaign">A campaign object holding at-least the name of the campaign </param>
    /// <param name="campaignCreatorUserId">user id of the campaign's creator.</param>
    /// <returns>id of the newly created campaign</returns>
    Task<int> AddCampaign(Campaign campaign, int? campaignCreatorUserId);
    /// <summary>
    /// Modifies a campaign in the database.
    /// </summary>
    /// <param name="campaign">A campaign object with the fields that should be modified set to not null</param>
    /// <returns></returns>
    Task ModifyCampaign(Campaign campaign);
    /// <summary>
    /// Gets the Guid of a campaign by its guid.
    /// </summary>
    /// <param name="campaignId"></param>
    /// <returns></returns>
    Task<Guid?> GetCampaignGuid(int? campaignId);
    /// <summary>
    /// Gets the guid and name of a campaign by the guid of its invite link.
    /// </summary>
    /// <param name="campaignInviteGuid"></param>
    /// <returns></returns>
    Task<Campaign?> GetCampaignByInviteGuid(Guid? campaignInviteGuid);
    /// <summary>
    /// Checks whether a user is in a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> IsUserInCampaign(Guid? campaignGuid, int? userId);

    /// <summary>
    /// Gets the type of a campaign from the database.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<CampaignType> GetCampaignType(Guid? campaignGuid);

    /// <summary>
    /// Gets the list of users in a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<User>> GetUsersInCampaign(Guid? campaignGuid);
    
    /// <summary>
    /// Deletes a campaign from the database.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task DeleteCampaign(Guid? campaignGuid);
}