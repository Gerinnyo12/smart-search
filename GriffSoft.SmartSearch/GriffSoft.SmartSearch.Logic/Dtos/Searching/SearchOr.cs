using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos.Searching;
public class SearchOr
{
    public required OrMatchType OrMatchType { get; init; }

    public required string FieldName { get; init; }

    public required string FieldValue { get; init; }
}
