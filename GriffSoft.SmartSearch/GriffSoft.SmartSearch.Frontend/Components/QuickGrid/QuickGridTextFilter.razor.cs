using Microsoft.AspNetCore.Components;

namespace GriffSoft.SmartSearch.Frontend.Components.QuickGrid;

public partial class QuickGridTextFilter : QuickGridComponent<string>
{
    [Parameter]
    public required string LabelName { get; set; }

    [Parameter]
    public required EventCallback OnValueChanged { get; set; }

    private async Task OnInputChangedAsync(ChangeEventArgs args)
    {
        await ValueChanged.InvokeAsync(GetStringValue(args));
        await OnValueChanged.InvokeAsync();
    }
}
