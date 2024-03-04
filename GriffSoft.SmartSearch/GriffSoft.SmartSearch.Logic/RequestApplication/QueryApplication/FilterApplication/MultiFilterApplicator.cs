using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.FilterApplication;
internal class MultiFilterApplicator
{
    private readonly MultiFilterDescriptor _multiFilterDescriptor;

    public MultiFilterApplicator(IEnumerable<SearchFilter> searchFilters)
    {
        _multiFilterDescriptor = new MultiFilterDescriptor(searchFilters);
    }

    public void ApplyFiltersOn(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        var filters = Filters;
        if (!filters.Any())
        {
            return;
        }

        boolQueryDescriptor.Filter(filters);
    }

    private Action<QueryDescriptor<ElasticDocument>>[] Filters =>
        _multiFilterDescriptor.CreateFilterDescriptors();
}
