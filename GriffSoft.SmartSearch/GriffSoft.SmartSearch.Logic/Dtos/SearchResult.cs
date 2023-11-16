using System.Linq;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class SearchResult<T>
{
    public required int HitCount { get; init; }

    public required IQueryable<T> Hits { get; init; }
}
