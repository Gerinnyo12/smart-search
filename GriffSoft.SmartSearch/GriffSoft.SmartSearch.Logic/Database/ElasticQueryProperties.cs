namespace GriffSoft.SmartSearch.Logic.Database;
internal class ElasticQueryProperties
{
    public required string[] Keys { get; init; }

    public required string Table { get; init; }

    public required string Column { get; init; }

    public required int BatchSize { get; init; }
}
