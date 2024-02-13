using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos.Synchronization;
public class ElasticDocumentMapperDto
{
    public required string Server { get; init; }

    public required string Database { get; init; }

    public required string Table { get; init; }

    public required TableType Type { get; init; }

    public required string[] Keys { get; init; }

    public required string[] Columns { get; init; }
}
