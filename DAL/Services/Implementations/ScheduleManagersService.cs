using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class ScheduleManagersService: IScheduleManagersService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public ScheduleManagersService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<(CustomStatusCode, IEnumerable<User>)> GetScheduleManagers(string? userEmail = null, int? userId = null)
    {
        var param = new DynamicParameters(new
        {
            userEmail,
            userId
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        var res = await _dbAccess.GetData<User, DynamicParameters>(StoredProcedureNames.GetScheduleManagers, param);
        
        return ((CustomStatusCode) param.Get<int>("returnVal"), res);
    }

    public async Task<CustomStatusCode> AddScheduleManager(int giverUserId, string receiverEmail)
    {
        var param = new DynamicParameters(new
        {
            giverUserId,
            receiverEmail
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AddScheduleManager, param);
        
        return (CustomStatusCode) param.Get<int>("returnVal");
    }
    
    public async Task<CustomStatusCode> RemoveScheduleManager(int giverUserId, string receiverEmail)
    {
        var param = new DynamicParameters(new
        {
            giverUserId,
            receiverEmail
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveScheduleManager, param);
        
        return (CustomStatusCode) param.Get<int>("returnVal");
    }

    public async Task<IEnumerable<UserPublicInfo>> GetManagedUsers(int userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        
        return await _dbAccess.GetData<UserPublicInfo, DynamicParameters>(StoredProcedureNames.GetManagedUsers, param);
    }
}