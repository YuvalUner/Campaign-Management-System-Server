using DAL.DbAccess;

namespace DAL.Services.Interfaces;

/// <summary>
/// A service for storing, retrieving, and modifying data related to campaign invites.
/// </summary>
public interface IInvitesService
{
    /// <summary>
    /// Creates an invite guid to a campaign
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to create an invite for.</param>
    /// <returns></returns>
    Task CreateInvite(Guid campaignGuid);

    /// <summary>
    /// Revokes the invite guid of a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to revoke the invite for</param>
    /// <returns></returns>
    Task RevokeInvite(Guid campaignGuid);

    /// <summary>
    /// Gets the invite guid of a campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to get an invite for</param>
    /// <returns>Guid of the invite</returns>
    Task<Guid?> GetInvite(Guid campaignGuid);

    /// <summary>
    /// Accepts an invite to a campaign and adds the user to the campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the invite belongs to.</param>
    /// <param name="userId">User id of the joining user.</param>
    /// <returns>Status code <see cref="CustomStatusCode.DuplicateKey"/> if user is already a member of the campaign.</returns>
    Task<CustomStatusCode> AcceptInvite(Guid? campaignGuid, int? userId);
}