using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Services;
using GriffSoft.SmartSearch.Logic.Settings;

using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Extensions.Options;

namespace GriffSoft.SmartSearch.Frontend.Providers;

public class SearchServiceProvider
{
    private readonly ElasticsearchService _elasticsearchService;

    public SearchServiceProvider(IOptions<IndexSettings> indexSettings,
        IOptions<ElasticsearchData> elasticsearchData,
        IOptions<ElasticClientSettings> elasticClientSettings)
    {
        _elasticsearchService = new ElasticsearchService(indexSettings.Value,
            elasticsearchData.Value, elasticClientSettings.Value);
    }

    public Task EnsureWorksAsync() => _elasticsearchService.EnsureAvailableAsync();

    public Task PrepareDataAsync() => _elasticsearchService.PrepareDataAsync();

    public async Task<IEnumerable<string>> SearchAsYouTypeQueryAsync(string query)
    {
        var sortDescriptor = new SortOptionsDescriptor<ElasticDocument>();
        sortDescriptor.Field(d => d.Value, new FieldSort
        {
            Order = SortOrder.Asc
        });

        const int firstTenHits = 10;
        var elasticsearchQuery = new ElasticsearchQuery<ElasticDocument>
        {
            Query = query,
            SortDescriptor = sortDescriptor,
            Size = firstTenHits,
            Offset = 0,
        };

        var searchResult = await _elasticsearchService.SearchAsync<ElasticDocument>(elasticsearchQuery);
        return searchResult.Hits
            .Where(h => h.Value != null)
            .Select(h => h.Value!.ToString()!);
    }

    public Task<SearchResult<ElasticDocument>> SearchAsync(GridItemsProviderRequest<ElasticDocument> request)
    {
        var sortProperties = request.GetSortByProperties();
        var sortProperty = sortProperties.First();
        string sortPropertyName = sortProperty.PropertyName;
        SortDirection sortPropertyDirection = sortProperty.Direction;

        var sortDescriptor = new SortOptionsDescriptor<ElasticDocument>();
        sortDescriptor.Field(sortPropertyName, new FieldSort
        {
            Order = sortPropertyDirection == SortDirection.Ascending ? SortOrder.Asc : SortOrder.Desc,
        });

        var searchQuery = new ElasticsearchQuery<ElasticDocument>
        {
            Query = "a",
            SortDescriptor = sortDescriptor,
            Size = request.Count ?? 10,
            Offset = request.StartIndex,
        };

        return _elasticsearchService.SearchAsync<ElasticDocument>(searchQuery);
    }
}
