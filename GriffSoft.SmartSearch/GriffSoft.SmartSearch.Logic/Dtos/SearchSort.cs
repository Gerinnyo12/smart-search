using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class SearchSort
{
    public required string FieldName { get; init; }

    public required SortDirection SortDirection { get; init; }

    public static implicit operator SearchSort(string fieldName) =>
        new()
        {
            FieldName = fieldName,
            SortDirection = SortDirection.Ascending,
        };
}
