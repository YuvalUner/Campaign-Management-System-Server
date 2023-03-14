using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;
using RestAPIServices;

namespace DAL.Services.Implementations;

public class NotificationsService : INotificationsService
{
    private readonly IGenericDbAccess _dbAccess;

    public NotificationsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task AddUserToNotify(int? userId, Guid campaignGuid, bool viaSms, bool viaEmail)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid,
            viaSms,
            viaEmail
        });
        await _dbAccess.ModifyData(StoredProcedureNames.ModifyUserToNotify, param);
    }
    
    public async Task RemoveUserToNotify(int? userId, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            userId,
            campaignGuid
        });
        await _dbAccess.ModifyData(StoredProcedureNames.ModifyUserToNotify, param);
    }
    
    public async Task UpdateUserToNotify(int? userId, Guid campaignGuid, bool viaSms, bool viaEmail)
    {
        // Due to the way the stored procedure UpdateUserToNotify is written, all 3 scenarios of add, update and 
        // remove are handled by the same stored procedure.
        // Therefore, this method is only a wrapper for AddUserToNotify, made for the sake of clarity.
        // Otherwise, places where you expect to modify a user will have to call a method called AddUserToNotify.
        await AddUserToNotify(userId, campaignGuid, viaSms, viaEmail);
    }
    
    public async Task<IEnumerable<NotificationSettings>> GetUsersToNotify(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        return await _dbAccess.GetData<NotificationSettings, DynamicParameters>
            (StoredProcedureNames.GetUsersToNotify, param);
    }
}