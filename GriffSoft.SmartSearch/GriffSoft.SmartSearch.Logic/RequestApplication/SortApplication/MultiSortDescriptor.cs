using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.SortApplication;
internal class MultiSortDescriptor
{
    private readonly IEnumerable<SearchSort> _searchSorts;

    public MultiSortDescriptor(IEnumerable<SearchSort> searchSorts)
    {
        _searchSorts = searchSorts;
    }

    public Action<SortOptionsDescriptor<ElasticDocument>>[] CreateSortDescriptors()
    {
        var validSearchSorts = _searchSorts.Where(ss => !string.IsNullOrWhiteSpace(ss.FieldName));
        if (!validSearchSorts.Any())
        {
            return Array.Empty<Action<SortOptionsDescriptor<ElasticDocument>>>();
        }

        var sortApplicators = new List<Action<SortOptionsDescriptor<ElasticDocument>>>();

        foreach (var searchSort in validSearchSorts)
        {
            var sortApplicator = new SortApplicator(searchSort);
            sortApplicators.Add(sortApplicator.ApplyOn);
        }

        return sortApplicators.ToArray();
    }
}
