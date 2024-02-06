using System;

namespace GriffSoft.SmartSearch.Database.Dtos;
public class SqlBatchUpsertQueryDto : SqlQueryDto
{
    public required DateTime LastSyncDate { get; init; }

    public required DateTime CurrentSyncDate { get; init; }

    public required string Table { get; init; }

    public required string[] Keys { get; init; }

    public required string[] Columns { get; init; }

    public required int BatchSize { get; init; }
}
