using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.AndApplication;
internal class MultiAndApplicator
{
    private readonly MultiAndDescriptor _multiAndDescriptor;

    public MultiAndApplicator(IEnumerable<SearchAnd> searchAnds)
    {
        _multiAndDescriptor = new MultiAndDescriptor(searchAnds);
    }

    public void ApplyAndsOn(BoolQueryDescriptor<ElasticDocument> boolQueryDescriptor)
    {
        var ands = Ands;
        if (!ands.Any())
        {
            return;
        }

        boolQueryDescriptor.Must(ands);
    }

    private Action<QueryDescriptor<ElasticDocument>>[] Ands =>
        _multiAndDescriptor.CreateAndDescriptors();
}
