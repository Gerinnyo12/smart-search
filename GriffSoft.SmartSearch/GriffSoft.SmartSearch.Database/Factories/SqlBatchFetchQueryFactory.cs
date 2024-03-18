using GriffSoft.SmartSearch.Database.Dtos;
using GriffSoft.SmartSearch.Database.Extensions;

using System.Linq;

namespace GriffSoft.SmartSearch.Database.Factories;
public class SqlBatchFetchQueryFactory : SqlBatchQueryFactory
{
    private const string SqlQuery =
        "SELECT {0} " +
        "FROM {1} " +
        "WITH (NOLOCK) " +
        "ORDER BY {2} " +
        "OFFSET {3} ROWS " +
        "FETCH NEXT {4} ROWS ONLY;";

    private readonly SqlBatchFetchQueryDto _sqlBatchFetchQueryDto;

    public SqlBatchFetchQueryFactory(SqlBatchFetchQueryDto sqlBatchFetchQueryDto) : base(sqlBatchFetchQueryDto)
    {
        _sqlBatchFetchQueryDto = sqlBatchFetchQueryDto;
    }

    protected override string GetParamaterisedSqlQuery()
    {
        string bracketisedColumnNames = string.Join(", ", _sqlBatchFetchQueryDto.Keys.Union(_sqlBatchFetchQueryDto.Columns).Select(s => s.Bracketise()));
        string bracketisedKeyNames = string.Join(", ", _sqlBatchFetchQueryDto.Keys.Select(s => s.Bracketise()));
        string bracketisedTableName = _sqlBatchFetchQueryDto.Table.Bracketise();
        int offset = _sqlBatchFetchQueryDto.BatchSize * _batchCount;
        int batchSize = _sqlBatchFetchQueryDto.BatchSize / _sqlBatchFetchQueryDto.Columns.Length;

        return string.Format(SqlQuery,
            bracketisedColumnNames,
            bracketisedTableName,
            bracketisedKeyNames,
            offset,
            batchSize);
    }
}
