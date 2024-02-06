using System;

namespace GriffSoft.SmartSearch.Logic.Options;
public class IndexOptions : IValidatable
{
    public string IndexName { get; init; } = "search-index";

    public int NumberOfShards { get; init; } = 3;

    public int NumberOfReplicas { get; init; } = 0;

    public void InvalidateIfIncorrect()
    {
        if (string.IsNullOrWhiteSpace(IndexName))
        {
            throw new Exception($"{nameof(IndexName)} must be provided.");
        }

        if (NumberOfShards <= 0)
        {
            throw new Exception($"{nameof(NumberOfShards)} must be provided and must not be negative.");
        }

        if (NumberOfReplicas < 0)
        {
            throw new Exception($"{nameof(NumberOfReplicas)} must be provided and must not be negative.");
        }
    }
}
