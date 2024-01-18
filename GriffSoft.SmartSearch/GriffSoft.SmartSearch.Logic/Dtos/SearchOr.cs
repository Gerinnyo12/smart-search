using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class SearchOr
{
    public SearchMatchType MatchType { get; init; } = SearchMatchType.Exact;

    public required string FieldName { get; init; }

    public required object FieldValue { get; init; }
}
