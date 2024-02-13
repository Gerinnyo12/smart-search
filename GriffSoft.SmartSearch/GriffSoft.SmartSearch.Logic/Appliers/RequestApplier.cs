using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

using SearchRequest = GriffSoft.SmartSearch.Logic.Dtos.Searching.SearchRequest;

namespace GriffSoft.SmartSearch.Logic.Appliers;
internal class RequestApplier
{
    private readonly SearchRequest _searchRequest;
    private readonly QueryApplier _queryApplier;
    private readonly SortApplier _sortApplier;

    public RequestApplier(SearchRequest searchRequest)
    {
        _searchRequest = searchRequest;
        _queryApplier = new QueryApplier(searchRequest.Filters, searchRequest.Ands, searchRequest.Ors);
        _sortApplier = new SortApplier(searchRequest.Sorts);
    }

    private void ApplyQuery(QueryDescriptor<ElasticDocument> queryDescriptor) =>
        _queryApplier.ApplyQuery(queryDescriptor);

    private void ApplySorts(SortOptionsDescriptor<ElasticDocument> sortDescriptor) =>
        _sortApplier.ApplySorts(sortDescriptor);

    public SearchRequestDescriptor<ElasticDocument> ApplyRequest(SearchRequestDescriptor<ElasticDocument> searchRequestDescriptor)
    {
        return searchRequestDescriptor
            .Query(ApplyQuery)
            .Sort(ApplySorts)
            .Size(_searchRequest.Size)
            .From(_searchRequest.Offset);
    }
}
