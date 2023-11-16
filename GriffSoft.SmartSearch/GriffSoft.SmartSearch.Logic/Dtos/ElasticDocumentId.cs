﻿using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Dtos;
internal class ElasticDocumentId
{
    public required string Server { get; init; }

    public required string Database { get; init; }

    public required string Table { get; init; }

    public required string Column { get; init; }

    public required Dictionary<string, object> Keys { get; init; }
}