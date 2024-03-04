using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.OrApplication;
internal class MultiOrApplicator
{
    private readonly MultiOrDescriptor _multiOrDescriptor;

    public MultiOrApplicator(IEnumerable<SearchOr> searchOrs)
    {
        _multiOrDescriptor = new MultiOrDescriptor(searchOrs);
    }

    public void ApplyOrsOn(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        var ors = Ors;
        if (!ors.Any())
        {
            return;
        }

        boolQueryDescriptor
            .Should(ors)
            .MinimumShouldMatch(1);
    }

    private Action<QueryDescriptor<ElasticDocument>>[] Ors =>
        _multiOrDescriptor.CreateOrDescriptors();
}
