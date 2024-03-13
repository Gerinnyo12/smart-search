using GriffSoft.SmartSearch.Logic.Dtos.Enums;

using Microsoft.AspNetCore.Components;

namespace GriffSoft.SmartSearch.Frontend.Components.QuickGrid;

public partial class QuickGridSearchBar : QuickGridComponent<FilterMatchType>
{
    public string _filter = string.Empty;

    [Parameter]
    public required int TotalCount { get; set; }

    [Parameter]
    public EventCallback<string> OnValueChanged { get; set; }

    private async Task OnFilterChangedAsync(FilterMatchType filterMatchType)
    {
        await ValueChanged.InvokeAsync(filterMatchType);
        await OnValueChanged.InvokeAsync(_filter);
    }

    private async Task OnInputChangedAsync(ChangeEventArgs args)
    {
        await OnValueChanged.InvokeAsync(GetStringValue(args));
    }
}
