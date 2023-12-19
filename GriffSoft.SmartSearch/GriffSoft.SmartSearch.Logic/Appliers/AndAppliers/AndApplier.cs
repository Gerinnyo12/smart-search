using Elastic.Clients.Elasticsearch.QueryDsl;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Extensions;

namespace GriffSoft.SmartSearch.Logic.Appliers.AndAppliers;
internal class AndApplier
{
    private readonly SearchAnd _searchAnd;

    public AndApplier(SearchAnd searchAnd)
    {
        _searchAnd = searchAnd;
    }

    public void And(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        var matchApplier = _searchAnd.MatchType.ToMatchApplier(_searchAnd.FieldName, _searchAnd.FieldValue);
        matchApplier.ApplyMatch(queryDescriptor);
    }
}
