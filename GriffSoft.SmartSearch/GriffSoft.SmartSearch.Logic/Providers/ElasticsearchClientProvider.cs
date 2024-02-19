using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Exceptions;
using GriffSoft.SmartSearch.Logic.Extensions;
using GriffSoft.SmartSearch.Logic.Factories;
using GriffSoft.SmartSearch.Logic.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Providers;
public class ElasticsearchClientProvider
{
    private ElasticsearchClient _elasticsearchClient = default!;

    private readonly ElasticsearchClientOptions _elasticsearchClientOptions;
    private readonly ILogger<ElasticsearchClientProvider> _logger;
    private readonly Task _initializationTask;

    public Task<ElasticsearchClient> Client => GetClientAsync();

    public ElasticsearchClientProvider(IOptions<ElasticsearchClientOptions> elasticsearchClientOptions, ILogger<ElasticsearchClientProvider> logger)
    {
        _elasticsearchClientOptions = elasticsearchClientOptions.Value;
        _logger = logger;

        _initializationTask = InitializeAsync();
    }

    private async Task<ElasticsearchClient> GetClientAsync()
    {
        await _initializationTask;
        return _elasticsearchClient;
    }

    private async Task InitializeAsync()
    {
        _elasticsearchClient = CreateClient();
        await EnsureAvailableAsync();
        await ReCreateIndexAsync();
    }

    private ElasticsearchClient CreateClient()
    {
        var elasticsearchClientSettingsFactory = new ElasticsearchClientSettingsFactory(_elasticsearchClientOptions);
        var elasticsearchClientSettings = elasticsearchClientSettingsFactory.Create();
        var elasticsearchClient = new ElasticsearchClient(elasticsearchClientSettings);

        return elasticsearchClient;
    }

    private async Task EnsureAvailableAsync()
    {
        var pingResponse = await _elasticsearchClient.PingAsync();
        if (!pingResponse.IsValidResponse)
        {
            string reason = pingResponse.GetExceptionMessage();
            var serverUnavailableException = new ServerUnavailableException(reason);
            _logger.LogError(serverUnavailableException, "Elastic client is not working.");
            throw serverUnavailableException;
        }
    }

    /// <summary>
    /// The index needs to be deleted first so every remnant unwanted data gets purged.
    /// </summary>
    private async Task ReCreateIndexAsync()
    {
        _logger.LogInformation("Recreating index '{indexName}'.", _elasticsearchClientOptions.IndexOptions.IndexName);
        await DropIndexAsync();
        await CreateIndexAsync();
    }

    private async Task DropIndexAsync()
    {
        var existsResponse = await _elasticsearchClient.Indices.ExistsAsync(_elasticsearchClientOptions.IndexOptions.IndexName);
        if (!existsResponse.Exists)
        {
            _logger.LogInformation("The {indexName} index does not exist yet.", _elasticsearchClientOptions.IndexOptions.IndexName);
            return;
        }

        var dropIndexRequestDescriptorFactory = new DropIndexRequestDescriptorFactory(_elasticsearchClientOptions.IndexOptions.IndexName);
        var dropIndexRequestDescriptor = dropIndexRequestDescriptorFactory.Create();

        var dropIndexResponse = await _elasticsearchClient.Indices.DeleteAsync(dropIndexRequestDescriptor);
        if (dropIndexResponse.IsValidResponse)
        {
            _logger.LogInformation("The {indexName} index was successfully deleted.", _elasticsearchClientOptions.IndexOptions.IndexName);
            return;
        }

        string reason = dropIndexResponse.GetExceptionMessage();
        var indexDropException = new IndexDropException(reason);
        _logger.LogError(indexDropException, "Could not drop index.");
        throw indexDropException;
    }

    private async Task CreateIndexAsync()
    {
        var createIndexRequestDescriptorFactory = new CreateIndexRequestDescriptorFactory(_elasticsearchClientOptions.IndexOptions);
        var createIndexDescriptor = createIndexRequestDescriptorFactory.Create();
        var createIndexResponse = await _elasticsearchClient.Indices.CreateAsync(createIndexDescriptor);
        if (createIndexResponse.IsValidResponse)
        {
            _logger.LogInformation("The {indexName} index was successfully created.", _elasticsearchClientOptions.IndexOptions.IndexName);
            return;
        }

        string reason = createIndexResponse.GetExceptionMessage();
        var indexCreationException = new IndexCreationException(reason);
        _logger.LogError(indexCreationException, "Could not create index.");
        throw indexCreationException;
    }
}
