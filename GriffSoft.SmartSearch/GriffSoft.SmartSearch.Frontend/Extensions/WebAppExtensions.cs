using GriffSoft.SmartSearch.Frontend.Providers;

namespace GriffSoft.SmartSearch.Frontend.Extensions;

public static class WebAppExtensions
{
    public static async Task InitializeAsync(this WebApplication webApp)
    {
        var serachServiceProvider = webApp.Services.GetRequiredService<SearchServiceProvider>();
        await serachServiceProvider.EnsureWorksAsync();
        await serachServiceProvider.PrepareDataAsync();
    }
}
