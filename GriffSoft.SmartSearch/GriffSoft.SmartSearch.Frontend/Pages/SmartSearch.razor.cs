using GriffSoft.SmartSearch.Frontend.Providers;
using GriffSoft.SmartSearch.Logic.Dtos;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace GriffSoft.SmartSearch.Frontend.Pages;

public partial class SmartSearch
{
    [Inject]
    private SearchServiceProvider? SearchServiceProvider { get; set;  }
    private QuickGrid<ElasticDocument>? Grid { get; set; }
    private GridItemsProvider<ElasticDocument>? ElasticDocumentsProvider { get; set; }
    private int TotalCount { get; set; }

    protected override void OnInitialized()
    {
        ElasticDocumentsProvider = async request =>
        {
            var result = await SearchServiceProvider!.SearchAsync(request);
            int totalCount = (int)result.TotalCount % int.MaxValue;

            if (totalCount != TotalCount)
            {
                TotalCount = totalCount;
                StateHasChanged();
            }

            return GridItemsProviderResult.From(result!.Hits.ToList(),totalCount);
        };

        base.OnInitialized();
    }
}
