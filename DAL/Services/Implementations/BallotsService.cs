using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;

public class BallotsService : IBallotsService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public BallotsService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<CustomStatusCode> AddCustomBallot(Ballot ballot, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            ballot.BallotAddress,
            ballot.BallotLocation,
            ballot.InnerCityBallotId,
            ballot.CityName,
            ballot.Accessible,
            ballot.ElligibleVoters,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.AddCustomBallot, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> UpdateCustomBallot(Ballot ballot, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            ballot.BallotAddress,
            ballot.BallotLocation,
            ballot.InnerCityBallotId,
            ballot.CityName,
            ballot.Accessible,
            ballot.ElligibleVoters,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.UpdateCustomBallot, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }
    
    public async Task<CustomStatusCode> DeleteCustomBallot(Decimal innerCityBallotId, Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            innerCityBallotId,
            campaignGuid
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.DeleteCustomBallot, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<IEnumerable<Ballot>> GetAllBallotsForCampaign(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        
        return await _dbAccess.GetData<Ballot, DynamicParameters>(StoredProcedureNames.GetAllCampaignBallots, param);
    }
}