using Microsoft.Data.SqlClient;

using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal class SqlConnector : IDatabaseConnector
{
    private readonly SqlConnection _sqlConnection;
    private readonly Task _connectionOpenerTask;

    public Task<DbConnection> Connection => GetConnectionAsync();

    public SqlConnector(string connectionString)
    {
        _sqlConnection = new SqlConnection(connectionString);
        _connectionOpenerTask = _sqlConnection.OpenAsync();
    }

    public async Task<DbConnection> GetConnectionAsync()
    {
        await _connectionOpenerTask;
        return _sqlConnection;
    }

    public void Dispose()
    {
        _sqlConnection.Dispose();
    }
}
