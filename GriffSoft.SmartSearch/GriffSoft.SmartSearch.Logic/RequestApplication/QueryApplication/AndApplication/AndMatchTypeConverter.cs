using GriffSoft.SmartSearch.Logic.Dtos.Enums;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;

using System;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.AndApplication;
internal class AndMatchTypeConverter
{
    private readonly SearchAnd _searchAnd;

    public AndMatchTypeConverter(SearchAnd searchAnd)
    {
        _searchAnd = searchAnd;
    }

    public MatchApplicator ToAndMatchApplicator() =>
        _searchAnd.AndMatchType switch
        {
            AndMatchType.Term => new TermMatchApplicator(_searchAnd.FieldName, _searchAnd.FieldValue),
            AndMatchType.Prefix => new PrefixMatchApplicator(_searchAnd.FieldName, _searchAnd.FieldValue),
            AndMatchType.Wildcard => new WildcardMatchApplicator(_searchAnd.FieldName, _searchAnd.FieldValue),
            _ => throw new Exception($"No query mapping found for {_searchAnd.AndMatchType}."),
        };
}
