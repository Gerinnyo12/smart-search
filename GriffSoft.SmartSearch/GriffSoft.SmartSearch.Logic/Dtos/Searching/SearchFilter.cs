using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos.Searching;
public class SearchFilter
{
    public required FilterMatchType FilterMatchType { get; init; }

    public required string FieldName { get; init; }

    public required string FieldValue { get; init; }
}
