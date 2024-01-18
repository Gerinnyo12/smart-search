namespace GriffSoft.SmartSearch.Database.Dtos;
public class SqlQueryDto
{
    public required string Table { get; init; }

    public required string[] Keys { get; init; }

    public required string Column { get; init; }
}
