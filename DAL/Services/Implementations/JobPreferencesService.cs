using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class JobPreferencesService : IJobPreferencesService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public JobPreferencesService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task AddUserPreferences(int? userId, Guid campaignGuid, string? userPreferencesText)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid,
        });
        if (userPreferencesText != null)
        {
            param.Add("userPreferencesText", userPreferencesText);
        }
        await _dbAccess.ModifyData(StoredProcedureNames.ModifyUserJobPreferences, param);
    }
    
    public async Task DeleteUserPreferences(int? userId, Guid campaignGuid)
    {
        // As ModifyUserJobPreferences is a stored procedure that handles all add, update and delete operations,
        // we can just pass in null for userPreferencesText and it will delete the record.
        // Therefore, this method is just a wrapper for AddUserPreferences, used for clarity.
        await AddUserPreferences(userId, campaignGuid, null);
    }
    
    public async Task UpdateUserPreferences(int? userId, Guid campaignGuid, string? userPreferencesText)
    {
        // As ModifyUserJobPreferences is a stored procedure that handles all add, update and delete operations,
        // we can just pass in the new value for userPreferencesText and it will update the record.
        // Therefore, this method is just a wrapper for AddUserPreferences, used for clarity.
        await AddUserPreferences(userId, campaignGuid, userPreferencesText);
    }
    
    public async Task<UserJobPreference?> GetUserPreferences(int? userId, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid,
        });
        var res = await _dbAccess.GetData<UserJobPreference, DynamicParameters>(StoredProcedureNames.GetUserJobPreferences, param);
        return res.FirstOrDefault();
    }
}