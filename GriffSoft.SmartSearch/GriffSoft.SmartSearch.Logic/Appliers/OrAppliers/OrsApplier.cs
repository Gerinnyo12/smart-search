using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.Appliers.OrAppliers;
internal class OrsApplier
{
    private readonly IEnumerable<SearchOr> _searchOrs;

    public OrsApplier(IEnumerable<SearchOr> searchOrs)
    {
        _searchOrs = searchOrs;
    }

    public void ApplyOrsIfNeeded(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        var validSearchOrs = _searchOrs.Where(so => !string.IsNullOrWhiteSpace(so.FieldValue?.ToString()));
        if (!validSearchOrs.Any())
        {
            return;
        }

        var ors = new List<Action<QueryDescriptor<ElasticDocument>>>();

        foreach (var searchOr in validSearchOrs)
        {
            var orApplier = new OrApplier(searchOr);
            ors.Add(orApplier.Or);
        }

        boolQueryDescriptor
            .Should(ors.ToArray())
            .MinimumShouldMatch(1);
    }
}
