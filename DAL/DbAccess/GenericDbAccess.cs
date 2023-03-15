using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DAL.DbAccess;

public class GenericDbAccess : IGenericDbAccess
{

    private readonly IConfiguration _config;

    public GenericDbAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<T>> GetData<T, U>(
        String storedProcedure,
        U parameters,
        String connectionId = "DefaultConnection")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

        return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task ModifyData<T>(String storedProcedure, T parameters, String connectionId = "DefaultConnection")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

        await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }
}