using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;

namespace ScheduleAPI.Data;

public class DataContextDapper
{
    private readonly IConfiguration _config;

    public DataContextDapper(IConfiguration config)
    {
        _config = config;
    }

    private DbConnection GetDbConnection()
    {
        return new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
    }

    public IEnumerable<T> Query<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        var dbConnection = GetDbConnection();
        return dbConnection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
    }

    public T QuerySingle<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        int? commandTimeout = null, CommandType? commandType = null)
    {
        var dbConnection = GetDbConnection();
        return dbConnection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType);
    }

    public int Execute(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var dbConnection = GetDbConnection();
        return dbConnection.Execute(sql, param, transaction, commandTimeout, commandType);
    }
}