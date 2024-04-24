using GriffSoft.SmartSearch.Database.Dtos;
using GriffSoft.SmartSearch.Database.Extensions;

using System;
using System.Linq;

namespace GriffSoft.SmartSearch.Database.Factories;
public class SqlBatchDeleteQueryFactory : SqlBatchQueryFactory
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
        ") a " +
        "WHERE RowNumber = 1 " +
        "AND ValidFrom <= {6} " +
        "AND ValidTo > {7} " +
        "AND ValidTo <= {8} " +
        "ORDER BY {9} " +
        "OFFSET {10} ROWS " +
        "FETCH NEXT {11} ROWS ONLY;";

    private readonly SqlBatchDeleteQueryDto _sqlDeleteQueryDto;

    public SqlBatchDeleteQueryFactory(SqlBatchDeleteQueryDto sqlDeleteQueryDto) : base(sqlDeleteQueryDto)
    {
        _sqlDeleteQueryDto = sqlDeleteQueryDto;
    }

    protected override string GetParamaterisedSqlQuery()
    {
        string bracketisedColumnNames = string.Join(", ", _sqlDeleteQueryDto.Keys.Union(_sqlDeleteQueryDto.Columns).Select(s => s.Bracketise()));
        string bracketisedKeyNames = string.Join(", ", _sqlDeleteQueryDto.Keys.Select(s => s.Bracketise()));
        string bracketisedTableName = _sqlDeleteQueryDto.Table.Bracketise();
        string quotisedIntervalStartDate = _sqlDeleteQueryDto.LastSyncDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff").Quotise();
        string quotisedIntervalEndDate = _sqlDeleteQueryDto.CurrentSyncDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff").Quotise();
        string quotiesedLastSyncDate = _sqlDeleteQueryDto.LastSyncDate > DateTime.MinValue ? quotisedIntervalStartDate : quotisedIntervalEndDate;
        int batchSize = _sqlDeleteQueryDto.BatchSize / _sqlDeleteQueryDto.Columns.Length;
        int offset = batchSize * _batchCount;

        return string.Format(SqlQuery,
            bracketisedColumnNames,
            bracketisedColumnNames,
            bracketisedKeyNames,
            bracketisedTableName,
            quotisedIntervalStartDate,
            quotisedIntervalEndDate,
            quotiesedLastSyncDate,
            quotisedIntervalStartDate,
            quotisedIntervalEndDate,
            bracketisedKeyNames,
            offset,
            batchSize);
    }
}
