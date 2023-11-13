namespace GriffSoft.SmartSearch.Logic.Dtos;
internal class ElasticMapperParameters
{
    public required string Server { get; init; }
    public required string Database { get; init; }
    public required string Table { get; init; }
    public required string Column { get; init; }
    public required string[] Keys { get; init; }
}
