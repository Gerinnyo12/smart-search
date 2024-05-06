using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Options;
using GriffSoft.SmartSearch.Logic.Providers;
using GriffSoft.SmartSearch.Logic.Services.Searching;
using GriffSoft.SmartSearch.Logic.Services.Synchronization;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GriffSoft.SmartSearch.Benchmark.DependecyInjection;
internal class ContainerFactory
{
    public ServiceProvider CreateConatiner()
    {
        string configPath = Path.Join(Directory.GetCurrentDirectory(), "appsettings.json");
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(configPath);
        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddOptions<ElasticsearchClientOptions>()
            .Bind(configuration.GetSection(nameof(ElasticsearchClientOptions)))
            .PostConfigure(e => e.InvalidateIfIncorrect());

        serviceCollection.AddOptions<ElasticsearchData>()
            .Bind(configuration.GetSection(nameof(ElasticsearchData)))
            .PostConfigure(e => e.InvalidateIfIncorrect());

        serviceCollection.AddOptions<FullTextSearchData>()
            .Bind(configuration.GetSection(nameof(FullTextSearchData)));

        serviceCollection.AddSingleton<ElasticsearchClientProvider>();
        serviceCollection.AddSingleton<ElasticSearchService>();

        serviceCollection.AddSingleton<ElasticBulkOperationService>();
        serviceCollection.AddSingleton<ElasticReIndexService>();

        serviceCollection.AddLogging();
        return serviceCollection.BuildServiceProvider();
    }
}
