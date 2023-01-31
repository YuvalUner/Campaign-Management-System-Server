using DAL.Models;

namespace DAL.Services.Interfaces;

public interface IJobPreferencesService
{
    Task AddUserPreferences(int? userId, Guid campaignGuid, string? userPreferencesText);
    Task DeleteUserPreferences(int? userId, Guid campaignGuid);
    Task UpdateUserPreferences(int? userId, Guid campaignGuid, string? userPreferencesText);
    Task<UserJobPreference?> GetUserPreferences(int? userId, Guid campaignGuid);
}