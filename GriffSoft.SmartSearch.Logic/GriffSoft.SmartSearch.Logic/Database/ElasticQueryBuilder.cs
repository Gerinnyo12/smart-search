using GriffSoft.SmartSearch.Logic.Dtos;

using Microsoft.Data.SqlClient;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal class ElasticQueryBuilder : IQueryBuilder
{
    private const string SqlCommand =
        "SELECT {0} " +
        "FROM {1} " +
        "WHERE {2} IS NOT NULL " +
        "AND {3} <> '' " +
        "ORDER BY {4} " +
        "OFFSET {5} ROWS " +
        "FETCH NEXT {6} ROWS ONLY;";

    private readonly IDatabaseConnector _databaseConnector;
    private readonly ElasticQueryParameters _elasticQueryParameters;

    public ElasticQueryBuilder(IDatabaseConnector databaseConnector, ElasticQueryParameters elasticQueryParameters)
    {
        _databaseConnector = databaseConnector;
        _elasticQueryParameters = elasticQueryParameters;
    }

    public async Task<DbCommand> BuildQueryForPage(int page)
    {
        var connection = await _databaseConnector.Connection;
        if (connection is not SqlConnection sqlConnection)
        {
            throw new Exception($"The provided connection is not an instance of {nameof(SqlConnection)}");
        }

        int offset = _elasticQueryParameters.BatchSize * page;
        string paramaterisedSqlQuery = ParamateriseSqlQuery(_elasticQueryParameters.BatchSize, offset);
        var sqlCommand = new SqlCommand
        {
            Connection = sqlConnection,
            CommandText = paramaterisedSqlQuery,
        };

        return sqlCommand;
    }

    private string ParamateriseSqlQuery(int batchSize, int offset)
    {
        string bracketisedCommaSeparatedKeysNames = string.Join(", ", _elasticQueryParameters.Keys.Select(Bracketise));
        string bracketisedColumnName = Bracketise(_elasticQueryParameters.Column);
        string bracketisedColumnNamesToSelect = $"{bracketisedCommaSeparatedKeysNames}, {bracketisedColumnName}";
        string bracketisedTableName = Bracketise(_elasticQueryParameters.Table);

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
