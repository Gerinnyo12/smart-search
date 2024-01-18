using GriffSoft.SmartSearch.Database.Connection.Sql;
using GriffSoft.SmartSearch.Database.Dtos;

using Microsoft.Data.SqlClient;

using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.Factories.Sql;
public class SqlQueryFactory : IQueryFactory<SqlCommand>
{
    private const string SqlQuery =
        "SELECT {0} " +
        "FROM {1} " +
        "WHERE {2} IS NOT NULL " +
        "AND {3} <> '' " +
        "ORDER BY {4} " +
        "OFFSET {5} ROWS " +
        "FETCH NEXT {6} ROWS ONLY;";

    private readonly SqlConnector _sqlConnector;
    private readonly SqlQueryDto _sqlQueryDto;

    public SqlQueryFactory(SqlConnector sqlConnector, SqlQueryDto sqlQueryDto)
    {
        _sqlConnector = sqlConnector;
        _sqlQueryDto = sqlQueryDto;
    }

    public async Task<SqlCommand> CreatePaginatedQueryAsync(int size, int page)
    {
        int offset = size * page;
        string paramaterisedSqlQuery = ParamateriseSqlQuery(size, offset);
        var sqlConnection = await _sqlConnector.Connection;

        var sqlCommand = new SqlCommand
        {
            Connection = sqlConnection,
            CommandText = paramaterisedSqlQuery,
        };

        return sqlCommand;
    }

    private string ParamateriseSqlQuery(int size, int offset)
    {
        string bracketisedCommaSeparatedKeyNames = string.Join(", ", _sqlQueryDto.Keys.Select(Bracketise));
        string bracketisedColumnNamesToSelect = string.Join(", ", _sqlQueryDto.Keys.Select(Bracketise));
        if (!bracketisedCommaSeparatedKeyNames.Contains(_sqlQueryDto.Column))
        {
            bracketisedColumnNamesToSelect = string.Join(", ", bracketisedCommaSeparatedKeyNames, Bracketise(_sqlQueryDto.Column));
        }

        string bracketisedTableName = Bracketise(_sqlQueryDto.Table);
        string bracketisedColumnName = Bracketise(_sqlQueryDto.Column);

        return string.Format(SqlQuery,
            bracketisedColumnNamesToSelect,
            bracketisedTableName,
            bracketisedColumnName,
            bracketisedColumnName,
            bracketisedCommaSeparatedKeyNames,
            offset,
            size);
    }

    private string Bracketise(string value) => $"[{value}]";
}
