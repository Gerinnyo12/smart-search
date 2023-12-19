using GriffSoft.SmartSearch.Logic.Builders;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Enums;
using GriffSoft.SmartSearch.Logic.Services;
using GriffSoft.SmartSearch.Logic.Settings;

using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Extensions.Options;

using SortDirection = GriffSoft.SmartSearch.Logic.Dtos.Enums.SortDirection;

namespace GriffSoft.SmartSearch.Frontend.Providers;

public class SearchServiceProvider
{
    public SearchFilter SearchFilter { get; set; } = string.Empty;

    public Dictionary<string, SearchAnd> SearchAnds { get; set;} = [];

    public Dictionary<TableType, SearchOr> SearchOrs { get; set; } = [];

    private readonly ElasticSearchService _elasticsearchService;

    public SearchServiceProvider(IOptions<IndexSettings> indexSettings,
        IOptions<ElasticsearchData> elasticsearchData,
        IOptions<ElasticClientSettings> elasticClientSettings)
    {
        _elasticsearchService = new ElasticSearchService(indexSettings.Value,
            elasticsearchData.Value, elasticClientSettings.Value);
    }

    public Task EnsureWorksAsync() => _elasticsearchService.EnsureAvailableAsync();

    public Task PrepareDataAsync() => _elasticsearchService.PrepareDataAsync();

    private static SortDirection GetSortDirection(Microsoft.AspNetCore.Components.QuickGrid.SortDirection direction) =>
        direction == Microsoft.AspNetCore.Components.QuickGrid.SortDirection.Ascending
            ? SortDirection.Ascending : SortDirection.Descending;

    public Task<SearchResult<ElasticDocument>> SearchAsync(GridItemsProviderRequest<ElasticDocument> request)
    {
        var searchAnds = SearchAnds.Values.ToArray();
        var searchOrs = SearchOrs.Values.ToArray();
        var searchSorts = request.GetSortByProperties()
            .Select(p => new SearchSort
            {
                FieldName = p.PropertyName,
                SortDirection = GetSortDirection(p.Direction),
            })
            .ToArray();
        
        var searchRequestBuilder = new SearchRequestBuilder();
        var searchRequest = searchRequestBuilder
            .Filter(SearchFilter)
            .Ands(searchAnds)
            .Ors(searchOrs)
            .Sorts(searchSorts)
            .Take(request.Count ?? 15)
            .Skip(request.StartIndex)
            .Build();

        return _elasticsearchService.SearchAsync(searchRequest);
    }
}
