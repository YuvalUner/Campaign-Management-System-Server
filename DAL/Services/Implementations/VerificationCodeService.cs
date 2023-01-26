using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;



public class VerificationCodeService : IVerificationCodeService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public VerificationCodeService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }
    
    public async Task CreateVerificationCode(int? userId, PhoneVerificationCode code)
    {
        var param = new DynamicParameters(new
        {
            userId,
            code.VerificationCode,
            code.PhoneNumber
        });
        await _dbAccess.ModifyData(StoredProcedureNames.AddVerificationCode, param);
    }
    
    public async Task<PhoneVerificationCode?> GetVerificationCode(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        var res = await _dbAccess.GetData<PhoneVerificationCode, DynamicParameters>
            (StoredProcedureNames.GetVerificationCode, param);
        return res.FirstOrDefault();
    }
    
    public async Task ApproveVerificationCode(int? userId, string? phoneNumber)
    {
        var param = new DynamicParameters(new
        {
            userId,
            phoneNumber
        });
        await _dbAccess.ModifyData(StoredProcedureNames.ApproveVerificationCode, param);
    }
}