using GriffSoft.SmartSearch.Logic.Dtos.Enums;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;

using System;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.OrApplication;
internal class OrMatchTypeConverter
{
    private readonly SearchOr _searchOr;

    public OrMatchTypeConverter(SearchOr searchOr)
    {
        _searchOr = searchOr;
    }

    public MatchApplicator ToOrMatchApplicator() =>
        _searchOr.OrMatchType switch
        {
            OrMatchType.Term => new TermMatchApplicator(_searchOr.FieldName, _searchOr.FieldValue),
            OrMatchType.Prefix => new PrefixMatchApplicator(_searchOr.FieldName, _searchOr.FieldValue),
            OrMatchType.Wildcard => new WildcardMatchApplicator(_searchOr.FieldName, _searchOr.FieldValue),
            _ => throw new Exception($"No query mapping found for {_searchOr.OrMatchType}."),
        };
}
