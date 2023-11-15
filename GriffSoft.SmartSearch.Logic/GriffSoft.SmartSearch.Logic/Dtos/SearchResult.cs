using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class SearchResult<T>
{
    public required int HitCount { get; init; }

    public required IEnumerable<T> Hits { get; init; }
}
