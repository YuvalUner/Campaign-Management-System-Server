using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class FinancialTypesService: IFinancialTypesService
{
    private readonly IGenericDbAccess _dbAccess;

    public FinancialTypesService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<(CustomStatusCode, Guid)> CreateFinancialType(FinancialType financialType)
    {
        var param = new DynamicParameters(new
        {
            financialType.TypeName,
            financialType.TypeDescription,
            financialType.CampaignGuid
        });

        param.Add("newTypeGuid", dbType: DbType.Guid, direction: ParameterDirection.Output);
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.AddFinancialType, param);

        return (param.Get<CustomStatusCode>("returnVal"), param.Get<Guid>("newTypeGuid"));
    }

    public async Task<CustomStatusCode> UpdateFinancialType(FinancialType financialType)
    {
        var param = new DynamicParameters(new
        {
            financialType.TypeName,
            financialType.TypeDescription,
            financialType.TypeGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.UpdateFinancialType, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<CustomStatusCode> DeleteFinancialType(Guid typeGuid)
    {
        var param = new DynamicParameters(new
        {
            typeGuid
        });

        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbAccess.ModifyData(StoredProcedureNames.DeleteFinancialType, param);

        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<IEnumerable<FinancialType>> GetFinancialTypes(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });


        var financialTypes =
            await _dbAccess.GetData<FinancialType, DynamicParameters>(StoredProcedureNames.GetFinancialTypesForCampaign,
                param);

        return financialTypes;
    }
}