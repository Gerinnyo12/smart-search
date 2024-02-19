using GriffSoft.SmartSearch.Frontend.Providers;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Enums;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace GriffSoft.SmartSearch.Frontend.Pages;

public partial class SmartSearch
{
    [Inject]
    private SearchServiceProvider? SearchServiceProvider { get; set; }

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

            return GridItemsProviderResult.From(result!.Hits.ToList(), totalCount);
        };

        base.OnInitialized();
    }

    private Task UpdateFilterAsync(string filter, SearchMatchType matchType = SearchMatchType.SearchAsYouType)
    {
        var searchFilter = new SearchFilter
        {
            Filter = filter,
            MatchType = matchType
        };

        SearchServiceProvider!.SearchFilter = searchFilter;
        return Grid!.RefreshDataAsync();
    }

    private Task UpdateAndAsync(string fieldName, string fieldValue, SearchMatchType matchType = SearchMatchType.Prefix)
    {
        var searchAnd = new SearchAnd
        {
            FieldName = fieldName,
            FieldValue = fieldValue,
            MatchType = matchType
        };

        SearchServiceProvider!.SearchAnds[searchAnd.FieldName] = searchAnd;
        return Grid!.RefreshDataAsync();
    }

    private Task UpdateOrAsync(string fieldName, string type, bool fieldValue)
    {
        if (fieldValue)
        {
            var searchOr = new SearchOr
            {
                FieldName = fieldName,
                FieldValue = type,
            };

            SearchServiceProvider!.SearchOrs[type] = searchOr;
        }
        else
        {
            SearchServiceProvider!.SearchOrs.Remove(type);
        }

        return Grid!.RefreshDataAsync();
    }

    private static string GetKeyList(Dictionary<string, object> keys)
    {
        var keyList = new List<string>();

        foreach (var key in keys)
        {
            keyList.Add(key.Key + ": " + key.Value);
        }

        return string.Join(", ", keyList);
    }
}
