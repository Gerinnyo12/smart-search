using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.Appliers.AndAppliers;
internal class AndsApplier
{
    private readonly IEnumerable<SearchAnd> _searchAnds;

    public AndsApplier(IEnumerable<SearchAnd> searchAnds)
    {
        _searchAnds = searchAnds;
    }

    public void ApplyAndsIfNeeded(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        var validSearchAnds = _searchAnds.Where(sa => !string.IsNullOrWhiteSpace(sa.FieldValue?.ToString()));
        if (!validSearchAnds.Any())
        {
            return;
        }

        var ands = new List<Action<QueryDescriptor<ElasticDocument>>>();

        foreach (var searchAnd in validSearchAnds)
        {
            var andApplier = new AndApplier(searchAnd);
            ands.Add(andApplier.And);
        }

        boolQueryDescriptor.Must(ands.ToArray());
    }
}
