using DAL.DbAccess;

namespace DAL.Services.Interfaces;

public interface IInvitesService
{
    /// <summary>
    /// Creates an invite guid to a campaign
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task CreateInvite(Guid campaignGuid);
    /// <summary>
    /// Revokes the invite guid of a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task RevokeInvite(Guid campaignGuid);
    /// <summary>
    /// Gets the invite guid of a campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    Task<Guid?> GetInvite(Guid campaignGuid);
    /// <summary>
    /// Accepts an invite to a campaign and adds the user to the campaign.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<CustomStatusCode> AcceptInvite(Guid? campaignGuid, int? userId);
}