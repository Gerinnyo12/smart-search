using Microsoft.Data.SqlClient;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.Connection.Sql;
public class SqlConnector : IDatabaseConnector<SqlConnection>
{
    private readonly SqlConnection _sqlConnection;
    private readonly Task _connectionOpenerTask;

    public Task<SqlConnection> Connection => GetConnectionAsync();

    public SqlConnector(string connectionString)
    {
        _sqlConnection = new SqlConnection(connectionString);
        _connectionOpenerTask = _sqlConnection.OpenAsync();
    }

    private async Task<SqlConnection> GetConnectionAsync()
    {
        await _connectionOpenerTask;
        return _sqlConnection;
    }

    public void Dispose()
    {
        _sqlConnection.Dispose();
    }
}
