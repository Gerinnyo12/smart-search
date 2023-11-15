using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.Extensions;
internal static class ElasticClientExtensions
{
    public static SearchResult<T> ToSearchResult<T>(this SearchResponse<T> searchResponse) where T : class
    {
        return new SearchResult<T>
        {
            HitCount = searchResponse.Documents.Count,
            Hits = searchResponse.Documents
        };
    }
}
