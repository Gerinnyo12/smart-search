using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.FilterApplication;
internal class MultiFilterDescriptor
{
    private readonly IEnumerable<SearchFilter> _searchFilters;

    public MultiFilterDescriptor(IEnumerable<SearchFilter> searchFilters)
    {
        _searchFilters = searchFilters;
    }

    public Action<QueryDescriptor<ElasticDocument>>[] CreateFilterDescriptors()
    {
        var validSearchFilters = _searchFilters.Where(sf => !string.IsNullOrWhiteSpace(sf.FieldValue));
        if (!validSearchFilters.Any())
        {
            return Array.Empty<Action<QueryDescriptor<ElasticDocument>>>();
        }

        var filterDescriptions = new List<Action<QueryDescriptor<ElasticDocument>>>();

        foreach (var searchFilter in validSearchFilters)
        {
            var filterApplicator = new FilterApplicator(searchFilter);
            filterDescriptions.Add(filterApplicator.ApplyFilterOn);
        }

        return filterDescriptions.ToArray();
    }
}
