using Microsoft.AspNetCore.Components;

namespace GriffSoft.SmartSearch.Frontend.Components.QuickGrid;

public abstract class QuickGridComponent<T> : ComponentBase
{
    [Parameter]
    public required T Value { get; set; }

    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }

    protected static string GetStringValue(ChangeEventArgs changeEventArgs) =>
        changeEventArgs.Value?.ToString() ?? string.Empty;
}
