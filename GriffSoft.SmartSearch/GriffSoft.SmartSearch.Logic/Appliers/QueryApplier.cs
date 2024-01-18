using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Appliers.AndAppliers;
using GriffSoft.SmartSearch.Logic.Appliers.OrAppliers;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Extensions;

using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Appliers;
internal class QueryApplier
{
    private readonly SearchFilter _searchFilter;
    private readonly AndsApplier _andsApplier;
    private readonly OrsApplier _orsApplier;

    public QueryApplier(SearchFilter searchFilter, IEnumerable<SearchAnd> searchAnds, IEnumerable<SearchOr> searchOrs)
    {
        _searchFilter = searchFilter;
        _andsApplier = new AndsApplier(searchAnds);
        _orsApplier = new OrsApplier(searchOrs);
    }

    private void ApplyFilterIfNeeded(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        if (string.IsNullOrWhiteSpace(_searchFilter.Filter))
        {
            return;
        }

        var matchApplier = _searchFilter.MatchType.ToMatchApplier(_searchFilter.Filter);
        var filter = matchApplier.ApplyMatch;
        boolQueryDescriptor.Filter(filter);
    }

    private void ApplyBoolQuery(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        ApplyFilterIfNeeded(boolQueryDescriptor);
        _andsApplier.ApplyAndsIfNeeded(boolQueryDescriptor);
        _orsApplier.ApplyOrsIfNeeded(boolQueryDescriptor);
    }

    public void ApplyQuery(QueryDescriptor<ElasticDocument> queryDescriptor) =>
        queryDescriptor.Bool(ApplyBoolQuery);
}
