using System.Data;
using DAL.DbAccess;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class PublishingService : IPublishingService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public PublishingService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<CustomStatusCode> PublishEvent(Guid? eventGuid, int? publisherId)
    {
        var param = new DynamicParameters(new
        {
            eventGuid,
            publisherId
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.PublishEvent, param);

        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> UnpublishEvent(Guid? eventGuid)
    {
        var param = new DynamicParameters(new
        {
            eventGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.UnpublishEvent, param);

        return param.Get<CustomStatusCode>("returnVal");
    }
}