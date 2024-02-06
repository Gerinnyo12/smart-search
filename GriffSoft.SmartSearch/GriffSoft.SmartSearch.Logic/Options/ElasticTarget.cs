using System;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.Options;

public class ElasticTarget : IValidatable
{
    public required string ConnectionString { get; init; }

    public required string Server { get; init; }

    public required string Database { get; init; }

    public required ElasticTargetTable[] Tables { get; init; }

    public void InvalidateIfIncorrect()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new Exception($"{nameof(ConnectionString)} must be provided.");
        }

        if (string.IsNullOrWhiteSpace(Server))
        {
            throw new Exception($"{nameof(Server)} must be provided.");
        }

        if (string.IsNullOrWhiteSpace(Database))
        {
            throw new Exception($"{nameof(Database)} must be provided.");
        }

        if (!Tables.Any())
        {
            throw new Exception("At least 1 table must be provided.");
        }

        foreach (var table in Tables)
        {
            table.InvalidateIfIncorrect();
        }
    }
}
