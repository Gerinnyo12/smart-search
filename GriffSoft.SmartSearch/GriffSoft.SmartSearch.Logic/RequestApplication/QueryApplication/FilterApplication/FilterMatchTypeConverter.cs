using GriffSoft.SmartSearch.Logic.Dtos.Enums;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;

using System;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.FilterApplication;
internal class FilterMatchTypeConverter
{
    private const string DefaultKeywordFieldPrefix = ".Raw";

    private readonly SearchFilter _searchFilter;

    public FilterMatchTypeConverter(SearchFilter searchFilter)
    {
        _searchFilter = searchFilter;
    }

    public MatchApplicator ToFilterMatchApplicator() =>
        _searchFilter.FilterMatchType switch
        {
            FilterMatchType.BoolPrefix => new BoolPrefixMatchApplicator(_searchFilter.FieldName, _searchFilter.FieldValue),
            FilterMatchType.PhrasePrefix => new PhrasePrefixMatchApplicator(_searchFilter.FieldName, _searchFilter.FieldValue),
            FilterMatchType.Wildcard => new WildcardMatchApplicator(_searchFilter.FieldName + DefaultKeywordFieldPrefix, _searchFilter.FieldValue),
            FilterMatchType.Prefix => new PrefixMatchApplicator(_searchFilter.FieldName + DefaultKeywordFieldPrefix, _searchFilter.FieldValue),
            FilterMatchType.Term => new TermMatchApplicator(_searchFilter.FieldName + DefaultKeywordFieldPrefix, _searchFilter.FieldValue),
            _ => throw new Exception($"No query mapping found for {_searchFilter.FilterMatchType}."),
        };
}
