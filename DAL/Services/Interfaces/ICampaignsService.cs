using DAL.Models;

namespace DAL.Services.Interfaces;

/// <summary>
/// A collection of methods used for storing, retrieving, and modifying data related to campaigns.
/// </summary>
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
    /// Gets the Guid of a campaign by its id.
    /// </summary>
    /// <param name="campaignId">Numerical id of the campaign</param>
    /// <returns>Guid of the campaign, if campaign with this id exists.</returns>
    Task<Guid?> GetCampaignGuid(int? campaignId);

    /// <summary>
    /// Gets the guid, whether the campaign is a custom campaign or not, and name of a campaign by the guid of its invite link.
    /// </summary>
    /// <param name="campaignInviteGuid"></param>
    /// <returns>The Guid, isCustomCampaign property and name of the campaign inside the returned
    /// <see cref="Campaign"/> object, if campaign with the invite Guid exists.</returns>
    Task<Campaign?> GetCampaignByInviteGuid(Guid? campaignInviteGuid);

    /// <summary>
    /// Checks whether a user is in a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <param name="userId">User id of the user to check.</param>
    /// <returns>True if the user is in the campaign, false otherwise.</returns>
    Task<bool> IsUserInCampaign(Guid? campaignGuid, int? userId);

    /// <summary>
    /// Gets the type of a campaign from the database.
    /// </summary>
    /// <param name="campaignGuid">The Guid of the campaign to check.</param>
    /// <returns>A <see cref="CampaignType"/> containing the campaign's type (municipal or not) as well as its city.</returns>
    Task<CampaignType> GetCampaignType(Guid? campaignGuid);

    /// <summary>
    /// Gets the list of users in a campaign.
    /// </summary>
    /// <param name="campaignGuid">The guid of the campaign.</param>
    /// <returns>A list of <see cref="UserInCampaign"/>, containing the public info and role info of every user.</returns>
    Task<IEnumerable<UserInCampaign>> GetUsersInCampaign(Guid? campaignGuid);

    /// <summary>
    /// Deletes a campaign from the database.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to delete.</param>
    /// <returns></returns>
    Task DeleteCampaign(Guid? campaignGuid);

    /// <summary>
    /// Gets a campaign name by its guid.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign that needs to have its name retrieved.</param>
    /// <returns></returns>
    Task<string?> GetCampaignNameByGuid(Guid? campaignGuid);

    /// <summary>
    /// Gets the basic info of a campaign.
    /// This includes the campaign's name, logo url, description, creation date, guid and type.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign.</param>
    /// <returns>A <see cref="Campaign"/> object containing the specified values, if the campaign exists.</returns>
    Task<Campaign?> GetCampaignBasicInfo(Guid? campaignGuid);
    
    /// <summary>
    /// Like <see cref="GetCampaignBasicInfo"/>, but gets the campaign by its invite guid instead of its own Guid.
    /// </summary>
    /// <param name="campaignInviteGuid">Invite guid of the campaign.</param>
    /// <returns></returns>
    Task<Campaign?> GetCampaignBasicInfoByInviteGuid(Guid? campaignInviteGuid);

    /// <summary>
    /// Gets a list of the admins of a campaign, including their role info and names.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<IEnumerable<CampaignAdminUserInfo>> GetCampaignAdmins(Guid? campaignGuid);
}