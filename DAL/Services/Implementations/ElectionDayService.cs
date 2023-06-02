using System.Data;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using Dapper;

namespace DAL.Services.Implementations;


public class ElectionDayService: IElectionDayService
{
    private readonly IGenericDbAccess _dbAccess;
    
    public ElectionDayService(IGenericDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public async Task<Ballot?> GetUserAssignedBallot(int? userId)
    {
        var param = new DynamicParameters(new
        {
            userId
        });
        
        var result = await _dbAccess.GetData<Ballot, DynamicParameters>(StoredProcedureNames.GetBallotForUser, param);
        
        return result.FirstOrDefault();
    }

    public async Task<CustomStatusCode> ModifyVoteCount(VoteCount voteCount, Guid campaignGuid, bool increment)
    {
        var param = new DynamicParameters(new
        {
            voteCount.IsCustomBallot,
            voteCount.BallotId,
            voteCount.PartyId,
            campaignGuid,
            increment
        });
        
        param.Add("returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        
        await _dbAccess.ModifyData(StoredProcedureNames.ModifyVoteCount, param);
        
        return param.Get<CustomStatusCode>("returnVal");
    }

    public async Task<IEnumerable<VoteCount>> GetVoteCount(Guid campaignGuid)
    {
        var param = new DynamicParameters(new
        {
            campaignGuid
        });
        
        return await _dbAccess.GetData<VoteCount, DynamicParameters>(StoredProcedureNames.GetVoteCount, param);
    }
}