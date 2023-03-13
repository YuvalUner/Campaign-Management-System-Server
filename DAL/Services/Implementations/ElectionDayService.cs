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
}