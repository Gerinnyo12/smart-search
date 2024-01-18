using GriffSoft.SmartSearch.Logic.Appliers.MatchAppliers;
using GriffSoft.SmartSearch.Logic.Dtos.Enums;

using System;

namespace GriffSoft.SmartSearch.Logic.Extensions;
internal static class SearchMatchTypeExtensions
{
    public static MatchApplier ToMatchApplier(this SearchMatchType searchMatchType, string? fieldName, object fieldValue) =>
        searchMatchType switch
        {
            SearchMatchType.SearchAsYouType => new SearchAsYouTypeMatchApplier(fieldName, fieldValue),
            SearchMatchType.Wildcard => new WildcardMatchApplier(fieldName, fieldValue),
            SearchMatchType.Prefix => new PrefixMatchApplier(fieldName, fieldValue),
            SearchMatchType.Exact => new ExactMatchApplier(fieldName, fieldValue),
            SearchMatchType.Numeric => new NumericOrMatchApplier(fieldName, fieldValue),
            _ => throw new Exception($"No query mapping found for {searchMatchType}."),
        };

    public static MatchApplier ToMatchApplier(this SearchMatchType searchMatchType, object fieldValue) =>
        searchMatchType.ToMatchApplier(null, fieldValue);
}
