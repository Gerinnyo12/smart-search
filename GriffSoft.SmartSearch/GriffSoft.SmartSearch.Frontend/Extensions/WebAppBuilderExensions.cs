using GriffSoft.SmartSearch.Logic.Options;

namespace GriffSoft.SmartSearch.Frontend.Extensions;

public static class WebAppBuilderExensions
{
    public static void RegisterValidatableOption<T>(this WebApplicationBuilder webAppBuilder, string sectionName) where T : class, IValidatable
    {
        webAppBuilder.Services.AddOptions<T>()
            .Bind(webAppBuilder.Configuration.GetSection(sectionName))
            .PostConfigure(t => t.InvalidateIfIncorrect());
    }
}
