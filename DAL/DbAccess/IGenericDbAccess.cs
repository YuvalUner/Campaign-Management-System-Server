namespace DAL.DbAccess;

public interface IGenericDbAccess
{
    /// <summary>
    /// Executes a stored procedure that returns a list of values of type T.
    /// SHOULD NOT modify any data.
    /// </summary>
    /// <typeparam name="T">The return type of the function</typeparam>
    /// <typeparam name="U">The type of the parameters</typeparam>
    /// <param name="storedProcedure">The stored procedure to execute</param>
    /// <param name="parameters">The parameters for the stored procedure</param>
    /// <param name="connectionId">The connection string's name from the appsettings.json file. Defaults to "DefaultConnection"</param>
    /// <returns>A list of type T retrieved from the database</returns>
    Task<IEnumerable<T>> GetData<T, U>(string storedProcedure, U parameters, string connectionId = "DefaultConnection");

    /// <summary>
    /// Executes a stored procedure that modifies data.
    /// Does not return any values by itself, but the queries can return values which should be picked up 
    /// using dynamic parameters.
    /// </summary>
    /// <typeparam name="T">The type of the parameters</typeparam>
    /// <param name="storedProcedure">The name of the stored proecedure to execute</param>
    /// <param name="parameters">The parameters for the stored procedure</param>
    /// <param name="connectionId">The connection string's name from the appsettings.json file. Defaults to "DefaultConnection"</param>
    /// <returns>Nothing</returns>
    Task ModifyData<T>(string storedProcedure, T parameters, string connectionId = "DefaultConnection");
}