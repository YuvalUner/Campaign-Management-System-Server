using System.Data;
using System.Dynamic;
using DAL.DbAccess;
using DAL.Models;
using Dapper;

namespace DAL.Services;

public class UsersService : IUsersService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public UsersService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<User?> GetUserByEmail(string? email)
    {
        var param = new DynamicParameters(new
        {
            Email = email
        });
        var res = await _dbAccess.GetData<User, DynamicParameters>(StoredProcedureNames.GetAllUserInfoByEmail, param);
        return res.FirstOrDefault();
    }
    
    public async Task<int> CreateUser(User user)
    {
        var param = new DynamicParameters(new
        {
            user.Email,
            user.FirstNameEng,
            user.LastNameEng,
            user.DisplayNameEng,
            user.ProfilePicUrl,
        });
        param.Add("userId", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.CreateUser, param);
        var userId = param.Get<int>("userId");
        return userId > 0 ? userId : -1;
    }

    public async Task<List<CampaignUser>> GetUserCampaigns(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        var res = await _dbAccess.GetData<CampaignUser, DynamicParameters>
            (StoredProcedureNames.GetUserCampaigns, param);
        return res.ToList();
    }

    public async Task<User?> GetUserPublicInfo(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        var res = await _dbAccess.GetData<User, DynamicParameters>
            (StoredProcedureNames.GetUserPublicInfoByUserID, param);
        return res.FirstOrDefault();
    }

    public async Task AddUserPrivateInfo(UserPrivateInfo privateInfo, int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId,
            privateInfo.FirstNameHeb,
            privateInfo.LastNameHeb,
            IdNum = privateInfo.IdNumber
        });
        await _dbAccess.ModifyData(StoredProcedureNames.AddUserPrivateInfo, param);
    }
}