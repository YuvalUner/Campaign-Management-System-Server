using System.Data;
using System.Dynamic;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

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
            (StoredProcedureNames.GetUserPublicInfoByUserId, param);
        return res.FirstOrDefault();
    }

    public async Task<CustomStatusCode> AddUserPrivateInfo(UserPrivateInfo privateInfo, int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId,
            privateInfo.FirstNameHeb,
            privateInfo.LastNameHeb,
            IdNum = privateInfo.IdNumber
        });
        param.Add("returnVal", DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dbAccess.ModifyData(StoredProcedureNames.AddUserPrivateInfo, param);
        return (CustomStatusCode) param.Get<int>("returnVal");
    }

    public async Task<bool> IsUserAuthenticated(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        var res = await _dbAccess.GetData<User, DynamicParameters>
            (StoredProcedureNames.GetUserAuthenticationStatus, param);
        return res.FirstOrDefault()?.Authenticated ?? false;
    }
    
    public async Task DeleteUser(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteUser, param);
    }

    public async Task AddPhoneNumber(int? userId, string phoneNumber)
    {
        var param = new DynamicParameters(new
        {
            userId,
            phoneNumber
        });
        await _dbAccess.ModifyData(StoredProcedureNames.AddUserPhoneNumber, param);
    }

    public async Task<User?> GetUserContactInfo(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        var res = await _dbAccess.GetData<User, DynamicParameters>
            (StoredProcedureNames.GetUserContactInfo, param);
        return res.FirstOrDefault();
    }

    public async Task<User?> GetUserContactInfoByEmail(string? userEmail)
    {
        var param = new DynamicParameters(new
        {
            userEmail
        });
        var res = await _dbAccess.GetData<User, DynamicParameters>
            (StoredProcedureNames.GetUserContactInfo, param);
        return res.FirstOrDefault();
    }

    public async Task RemovePhoneNumber(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        await _dbAccess.ModifyData(StoredProcedureNames.RemoveUserPhoneNumber, param);
    }
}