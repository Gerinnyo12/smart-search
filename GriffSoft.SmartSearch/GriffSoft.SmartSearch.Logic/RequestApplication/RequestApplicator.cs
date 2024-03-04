using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication;
using GriffSoft.SmartSearch.Logic.RequestApplication.SortApplication;

using System;

using SearchRequest = GriffSoft.SmartSearch.Logic.Dtos.Searching.SearchRequest;

namespace GriffSoft.SmartSearch.Logic.RequestApplication;
internal class RequestApplicator
{
    private readonly QueryApplicator _queryApplicator;
    private readonly MultiSortDescriptor _multiSortDescriptor;
    private readonly int _requestSize;
    private readonly int _requestOffset;

    public RequestApplicator(SearchRequest searchRequest)
    {
        _queryApplicator = new QueryApplicator(searchRequest.Filters, searchRequest.Ands, searchRequest.Ors);
        _multiSortDescriptor = new MultiSortDescriptor(searchRequest.Sorts);
        _requestSize = searchRequest.Size;
        _requestOffset = searchRequest.Offset;
    }

    public void ApplyRequestOn(SearchRequestDescriptor<ElasticDocument> searchRequestDescriptor)
    {
        searchRequestDescriptor
            .Query(ApplyQuery)
            .Sort(Sorts)
            .Size(_requestSize)
            .From(_requestOffset);
    }

    private void ApplyQuery(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        _queryApplicator.ApplyQueryOn(queryDescriptor);
    }

    private Action<SortOptionsDescriptor<ElasticDocument>>[] Sorts =>
        _multiSortDescriptor.CreateSortDescriptors();
}
