using Microsoft.Data.SqlClient;

using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal class ElasticQueryBuilder : IDatabaseQueryBuilder<SqlCommand>
{
    private const string SqlCommand =
        "SELECT {0} " +
        "FROM {1} " +
        "WHERE {2} IS NOT NULL " +
        "AND {3} <> '' " +
        "ORDER BY {4} " +
        "OFFSET {5} ROWS " +
        "FETCH NEXT {6} ROWS ONLY;";

    private readonly SqlConnector _sqlConnector;
    private readonly ElasticQueryProperties _elasticQueryProperties;

    public ElasticQueryBuilder(SqlConnector sqlConnector, ElasticQueryProperties elasticQueryProperties)
    {
        _sqlConnector = sqlConnector;
        _elasticQueryProperties = elasticQueryProperties;
    }

    public async Task<SqlCommand> BuildQueryForPageAsync(int page)
    {
        var sqlConnection = await _sqlConnector.Connection;
        int offset = _elasticQueryProperties.BatchSize * page;
        string paramaterisedSqlQuery = ParamateriseSqlQuery(_elasticQueryProperties.BatchSize, offset);
        var sqlCommand = new SqlCommand
        {
            Connection = sqlConnection,
            CommandText = paramaterisedSqlQuery,
        };

        return sqlCommand;
    }

    private string ParamateriseSqlQuery(int batchSize, int offset)
    {
        string bracketisedCommaSeparatedKeysNames = string.Join(", ", _elasticQueryProperties.Keys.Select(Bracketise));
        string bracketisedColumnName = Bracketise(_elasticQueryProperties.Column);
        string bracketisedColumnNamesToSelect = $"{bracketisedCommaSeparatedKeysNames}, {bracketisedColumnName}";
        string bracketisedTableName = Bracketise(_elasticQueryProperties.Table);

        return string.Format(SqlCommand,
            bracketisedColumnNamesToSelect,
            bracketisedTableName,
            bracketisedColumnName,
            bracketisedColumnName,
            bracketisedCommaSeparatedKeysNames,
            offset,
            batchSize);
    }

    private string Bracketise(string value) => $"[{value}]";
}
