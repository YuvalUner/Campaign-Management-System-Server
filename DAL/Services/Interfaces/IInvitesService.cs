namespace DAL.Services.Interfaces;

public interface IInvitesService
{
    Task CreateInvite(Guid campaignGuid);
    Task RevokeInvite(Guid campaignGuid);
    Task<Guid?> GetInvite(Guid campaignGuid);
}