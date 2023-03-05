using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;


public class UsersPublicBoardPreferenceService : IUsersPublicBoardPreferenceService
{
    private readonly IGenericDbAccess _dbAccess;

    public UsersPublicBoardPreferenceService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<CustomStatusCode> AddPreference(int? userId, Guid campaignGuid, bool isPreferred)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid,
            isPreferred
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.AddUserPreference, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> RemovePreference(int? userId, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.DeleteUserPreference, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> UpdatePreference(int? userId, Guid campaignGuid, bool isPreferred)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid,
            isPreferred
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.UpdateUserPreference, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<(CustomStatusCode, IEnumerable<UserPreferenceResult>)> GetPreferences(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        var result =
            await _dbAccess.GetData<UserPreferenceResult, DynamicParameters>(StoredProcedureNames.GetUserPreferences,
                param);

        return (param.Get<CustomStatusCode>("returnVal"), result);
    }
}