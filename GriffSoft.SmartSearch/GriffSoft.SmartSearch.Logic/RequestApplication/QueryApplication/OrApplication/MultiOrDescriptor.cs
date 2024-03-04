using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.OrApplication;
internal class MultiOrDescriptor
{
    private readonly IEnumerable<SearchOr> _searchOrs;

    public MultiOrDescriptor(IEnumerable<SearchOr> searchOrs)
    {
        _searchOrs = searchOrs;
    }

    public Action<QueryDescriptor<ElasticDocument>>[] CreateOrDescriptors()
    {
        var validSearchOrs = _searchOrs.Where(so => !string.IsNullOrWhiteSpace(so.FieldValue));
        if (!validSearchOrs.Any())
        {
            return Array.Empty<Action<QueryDescriptor<ElasticDocument>>>();
        }

        var orDescriptors = new List<Action<QueryDescriptor<ElasticDocument>>>();

        foreach (var searchOr in validSearchOrs)
        {
            var orApplicator = new OrApplicator(searchOr);
            orDescriptors.Add(orApplicator.ApplyOrOn);
        }

        return orDescriptors.ToArray();
    }
}
