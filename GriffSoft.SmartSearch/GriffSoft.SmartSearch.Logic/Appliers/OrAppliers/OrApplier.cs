using Elastic.Clients.Elasticsearch.QueryDsl;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Extensions;

namespace GriffSoft.SmartSearch.Logic.Appliers.OrAppliers;
internal class OrApplier
{
    private readonly SearchOr _searchOr;

    public OrApplier(SearchOr searchOr)
    {
        _searchOr = searchOr;
    }

    public void Or(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        var matchApplier = _searchOr.MatchType.ToMatchApplier(_searchOr.FieldName, _searchOr.FieldValue);
        matchApplier.ApplyMatch(queryDescriptor);
    }
}
