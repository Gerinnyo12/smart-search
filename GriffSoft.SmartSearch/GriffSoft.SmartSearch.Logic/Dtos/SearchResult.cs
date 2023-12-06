using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class SearchResult<T>
{
    public required long TotalCount { get; init; }

    public required IReadOnlyCollection<T> Hits { get; init; }
}
