using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.AndApplication;
using GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.FilterApplication;
using GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.OrApplication;

using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication;
internal class QueryApplicator
{
    private readonly MultiFilterApplicator _multiFilterApplicator;
    private readonly MultiAndApplicator _multiAndsApplicator;
    private readonly MultiOrApplicator _multiOrApplicator;

    public QueryApplicator(IEnumerable<SearchFilter> searchFilters, IEnumerable<SearchAnd> searchAnds, IEnumerable<SearchOr> searchOrs)
    {
        _multiFilterApplicator = new MultiFilterApplicator(searchFilters);
        _multiAndsApplicator = new MultiAndApplicator(searchAnds);
        _multiOrApplicator = new MultiOrApplicator(searchOrs);
    }

    public void ApplyQueryOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor.Bool(ApplyBoolQuery);
    }

    private void ApplyBoolQuery(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        _multiFilterApplicator.ApplyFiltersOn(boolQueryDescriptor);
        _multiAndsApplicator.ApplyAndsOn(boolQueryDescriptor);
        _multiOrApplicator.ApplyOrsOn(boolQueryDescriptor);
    }
}
