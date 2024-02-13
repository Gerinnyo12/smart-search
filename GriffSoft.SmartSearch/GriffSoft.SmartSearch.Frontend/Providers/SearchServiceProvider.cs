using GriffSoft.SmartSearch.Logic.Builders;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.Services.Searching;

using Microsoft.AspNetCore.Components.QuickGrid;

using SortDirection = GriffSoft.SmartSearch.Logic.Dtos.Enums.SortDirection;

namespace GriffSoft.SmartSearch.Frontend.Providers;

public class SearchServiceProvider(ISearchService<ElasticDocument> elasticsearchService)
{
    public SearchFilter SearchFilter { get; set; } = string.Empty;

    public Dictionary<string, SearchAnd> SearchAnds { get; set; } = [];

    public Dictionary<string, SearchOr> SearchOrs { get; set; } = [];

    private readonly ISearchService<ElasticDocument> _elasticsearchService = elasticsearchService;

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
