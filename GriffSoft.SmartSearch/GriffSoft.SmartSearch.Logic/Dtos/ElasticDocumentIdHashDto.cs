using GriffSoft.SmartSearch.Logic.Dtos.Enums;

using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Dtos;
internal class ElasticDocumentIdHashDto
{
    public required string Server { get; init; }

    public required string Database { get; init; }

    public required string Table { get; init; }

    public required TableType Type { get; init; }

    public required string Column { get; init; }

    public required Dictionary<string, object> Keys { get; init; }
}
