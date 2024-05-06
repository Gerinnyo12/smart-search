namespace GriffSoft.SmartSearch.Benchmark;
internal class FullTextSearchData
{
    public required string ConnectionString { get; init; }

    public required string Table { get; init; }

    public required string Column { get; init; }

    public required string Id { get; init; }

    public required int Offset { get; init; }

    public required int Size { get; init; }
}
