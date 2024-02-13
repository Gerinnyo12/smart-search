using GriffSoft.SmartSearch.Database.Connection;
using GriffSoft.SmartSearch.Logic.Options;

using System;

namespace GriffSoft.SmartSearch.Logic.Dtos.Synchronization;
public class ElasticSynchronizationDto
{
    public required SqlConnector SqlConnector { get; init; }

    public required ElasticTarget Target { get; init; }

    public required ElasticTargetTable Table { get; init; }

    public required DateTime LastSynchronizationDate { get; init; }

    public required DateTime SynchronizationDate { get; init; }

    public required int BatchSize { get; init; }
}
