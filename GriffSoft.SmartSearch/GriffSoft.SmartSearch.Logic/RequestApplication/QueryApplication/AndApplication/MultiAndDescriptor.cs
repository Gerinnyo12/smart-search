using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.AndApplication;
internal class MultiAndDescriptor
{
    private readonly IEnumerable<SearchAnd> _searchAnds;

    public MultiAndDescriptor(IEnumerable<SearchAnd> searchAnds)
    {
        _searchAnds = searchAnds;
    }

    public Action<QueryDescriptor<ElasticDocument>>[] CreateAndDescriptors()
    {
        var validSearchAnds = _searchAnds.Where(sa => !string.IsNullOrWhiteSpace(sa.FieldValue));
        if (!validSearchAnds.Any())
        {
            return Array.Empty<Action<QueryDescriptor<ElasticDocument>>>();
        }

        var andApplicators = new List<Action<QueryDescriptor<ElasticDocument>>>();

        foreach (var searchAnd in validSearchAnds)
        {
            var andApplicator = new AndApplicator(searchAnd);
            andApplicators.Add(andApplicator.ApplyAndOn);
        }

        return andApplicators.ToArray();
    }
}
