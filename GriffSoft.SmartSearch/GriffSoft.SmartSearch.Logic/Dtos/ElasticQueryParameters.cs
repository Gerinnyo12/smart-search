namespace GriffSoft.SmartSearch.Logic.Dtos;
internal class ElasticQueryParameters
{
    public required string[] Keys { get; init; }

    public required string Table { get; init; }

    public required string Column { get; init; }

    public required int BatchSize { get; init; }
}
