using System.Data;
using DAL.DbAccess;
using DAL.Models;
using Dapper;

namespace DAL.Services.Implementations;

public class ScheduleManagersService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public ScheduleManagersService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<IEnumerable<User>> GetScheduleManagers(string userEmail)
    {
        var param = new DynamicParameters(new
        {
            userEmail
        });

        return await _dbAccess.GetData<User, DynamicParameters>(StoredProcedureNames.GetScheduleManagers, param);
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
}