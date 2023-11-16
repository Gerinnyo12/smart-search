using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Services;
using GriffSoft.SmartSearch.Logic.Settings;

using Microsoft.Extensions.Options;

namespace GriffSoft.SmartSearch.Frontend.Providers;

public class SearchServiceProvider
{
    private readonly ISearchService _elasticsearchService;

    public SearchServiceProvider(IOptions<IndexSettings> indexSettings,
        IOptions<ElasticsearchData> elasticsearchData, 
        IOptions<ElasticClientSettings> elasticClientSettings)
    {
        _elasticsearchService = new ElasticsearchService(indexSettings.Value,
            elasticsearchData.Value, elasticClientSettings.Value);
    }

    public Task EnsureWorksAsync() => _elasticsearchService.EnsureAvailableAsync();

    public Task PrepareDataAsync() => _elasticsearchService.PrepareDataAsync();

    public async Task<IQueryable<string>> SearchAsYouTypeQueryAsync(string query)
    {
        const int firstTenHits = 10;
        var searchQuery = new PaginatedSearchQuery
        {
            Query = query,
            Size = firstTenHits,
            Offset = 0,
        };

        var searchResult = await _elasticsearchService.SearchAsync<ElasticDocument>(searchQuery);
        return searchResult.Hits
            .Where(h => h.Value != null)
            .Select(h => h.Value!.ToString()!);
    }

    public Task<SearchResult<ElasticDocument>> SearchAsync(PaginatedSearchQuery searchQuery)
    {
        return _elasticsearchService.SearchAsync<ElasticDocument>(searchQuery);
    }
}
