namespace GriffSoft.SmartSearch.Logic.Dtos;
public class PaginatedSearchQuery
{
    public required string Query { get; init; }

    public required int Size { get; init; }

    public required int Offset { get; init; }
}
