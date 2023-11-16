namespace GriffSoft.SmartSearch.Frontend.Extensions;

public static class WebAppBuilderExensions
{
    public static void RegisterOption<T>(this WebApplicationBuilder webAppBuilder, string sectionName) where T : class
    {
        webAppBuilder.Services.AddOptions<T>()
            .Bind(webAppBuilder.Configuration.GetSection(sectionName));
    }
}
