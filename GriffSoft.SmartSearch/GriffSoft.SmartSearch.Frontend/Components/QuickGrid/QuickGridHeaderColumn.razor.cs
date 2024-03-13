using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace GriffSoft.SmartSearch.Frontend.Components.QuickGrid;

public partial class QuickGridHeaderColumn
{
    [Parameter]
    public required string Title { get; set; }

    [Parameter]
    public required SortDirection? SortDirection { get; set; }

    [Parameter]
    public required EventCallback OnSortClicked { get; set; }

    [Parameter]
    public required EventCallback OnFilterClicked { get; set; }

    private Task OnSortButtonClicked()
    {
        return OnSortClicked.InvokeAsync();
    }

    private Task OnFilterButtonClicked()
    {
        return OnFilterClicked.InvokeAsync();
    }
}
