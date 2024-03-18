using GriffSoft.SmartSearch.Database.Dtos;
using GriffSoft.SmartSearch.Database.Extensions;

using System.Linq;

namespace GriffSoft.SmartSearch.Database.Factories;
public class SqlBatchUpsertQueryFactory : SqlBatchQueryFactory
{
    private const string SqlQuery =
        "SELECT {0} " +
        "FROM ( " +
        "   SELECT {1}, ValidFrom, ValidTo, ROW_NUMBER() OVER ( " +
        "       PARTITION BY {2} " +
        "       ORDER BY ValidTo DESC " +
        "   ) RowNumber " +
        "   FROM {3} " +
        "   FOR SYSTEM_TIME " +
        "   BETWEEN {4} AND {5} " +
        "   WITH (NOLOCK)" +
        ") a " +
        "WHERE RowNumber = 1 " +
        "AND ValidFrom > {6} " +
        "AND ValidFrom <= {7} " +
        "AND ValidTo > {8} " +
        "ORDER BY {9} " +
        "OFFSET {10} ROWS " +
        "FETCH NEXT {11} ROWS ONLY;";

    private readonly SqlBatchUpsertQueryDto _sqlUpsertQueryDto;

    public SqlBatchUpsertQueryFactory(SqlBatchUpsertQueryDto sqlUpsertQueryDto) : base(sqlUpsertQueryDto)
    {
        _sqlUpsertQueryDto = sqlUpsertQueryDto;
    }

    protected override string GetParamaterisedSqlQuery()
    {
        string bracketisedColumnNames = string.Join(", ", _sqlUpsertQueryDto.Keys.Union(_sqlUpsertQueryDto.Columns).Select(s => s.Bracketise()));
        string bracketisedKeyNames = string.Join(", ", _sqlUpsertQueryDto.Keys.Select(s => s.Bracketise()));
        string bracketisedTableName = _sqlUpsertQueryDto.Table.Bracketise();
        string quotisedIntervalStartDate = _sqlUpsertQueryDto.LastSyncDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff").Quotise();
        string quotisedIntervalEndDate = _sqlUpsertQueryDto.CurrentSyncDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff").Quotise();
        int offset = _sqlUpsertQueryDto.BatchSize * _batchCount;
        int batchSize = _sqlUpsertQueryDto.BatchSize / _sqlUpsertQueryDto.Columns.Length;

        return string.Format(SqlQuery,
            bracketisedColumnNames,
            bracketisedColumnNames,
            bracketisedKeyNames,
            bracketisedTableName,
            quotisedIntervalStartDate,
            quotisedIntervalEndDate,
            quotisedIntervalStartDate,
            quotisedIntervalEndDate,
            quotisedIntervalEndDate,
            bracketisedKeyNames,
            offset,
            batchSize);
    }
}
