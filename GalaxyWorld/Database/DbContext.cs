using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace GalaxyWorld.Database;

public class DbContext(IOptions<DbOptions> options) : IDisposable
{
    private IDbConnection? _connection;

    private IDbConnection GetConnection() {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new NpgsqlConnection(options.Value.DbConnectionString);
            _connection.Open();
        }
        return _connection;
    }

    public IDbConnection Connection => GetConnection();

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
