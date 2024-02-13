using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos.Searching;

namespace GriffSoft.SmartSearch.Logic.Extensions;
internal static class ElasticClientExtensions
{
    public static SearchResult<T> ToSearchResult<T>(this SearchResponse<T> searchResponse) where T : class
    {
        return new SearchResult<T>
        {
            TotalCount = searchResponse.Total,
            Hits = searchResponse.Documents,
        };
    }

    public static string GetExceptionMessage(this ElasticsearchResponse elasticsearchResponse)
    {
        const string UnknownReason = "Unknown reason";
        string reason = elasticsearchResponse.ElasticsearchServerError?.Error?.Reason
            ?? elasticsearchResponse.ApiCallDetails.OriginalException?.Message
            ?? UnknownReason;

        return reason;
    }
}
