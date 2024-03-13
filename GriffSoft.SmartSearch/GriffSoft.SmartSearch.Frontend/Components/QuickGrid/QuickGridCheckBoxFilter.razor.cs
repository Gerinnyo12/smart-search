using Microsoft.AspNetCore.Components;

namespace GriffSoft.SmartSearch.Frontend.Components.QuickGrid;

public partial class QuickGridCheckBoxFilter : QuickGridComponent<bool>
{
    [Parameter]
    public required string CheckBoxName { get; set; }

    [Parameter]
    public required EventCallback OnValueChanged { get; set; }

    private async Task OnCheckBoxChangedAsync(ChangeEventArgs args)
    {
        await ValueChanged.InvokeAsync((bool?)args.Value ?? false);
        await OnValueChanged.InvokeAsync();
    }
}
