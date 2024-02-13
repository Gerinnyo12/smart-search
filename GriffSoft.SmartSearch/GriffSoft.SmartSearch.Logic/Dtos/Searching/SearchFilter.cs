using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos.Searching;
public class SearchFilter
{
    public required SearchMatchType MatchType { get; init; }

    public required string Filter { get; init; }

    public static implicit operator SearchFilter(string filter) =>
        new()
        {
            Filter = filter,
            MatchType = SearchMatchType.Wildcard
        };
}
