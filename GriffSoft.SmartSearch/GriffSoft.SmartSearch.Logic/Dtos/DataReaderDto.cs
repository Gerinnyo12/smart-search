using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class DataReaderDto
{
    public required string Server { get; init; }

    public required string Database { get; init; }

    public required string Table { get; init; }

    public required TableType Type { get; init; }

    public required string Column { get; init; }

    public required string[] Keys { get; init; }
}
