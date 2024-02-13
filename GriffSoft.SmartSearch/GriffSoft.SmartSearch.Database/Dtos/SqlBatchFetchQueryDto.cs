﻿namespace GriffSoft.SmartSearch.Database.Dtos;
public class SqlBatchFetchQueryDto : SqlQueryDto
{
    public required string Table { get; init; }

    public required string[] Keys { get; init; }

    public required string[] Columns { get; init; }

    public required int BatchSize { get; init; }
}
