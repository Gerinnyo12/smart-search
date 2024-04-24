using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Synchronization;
using GriffSoft.SmartSearch.Logic.Exceptions;
using GriffSoft.SmartSearch.Logic.Extensions;
using GriffSoft.SmartSearch.Logic.Factories;
using GriffSoft.SmartSearch.Logic.Options;
using GriffSoft.SmartSearch.Logic.Providers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services.Synchronization;
public class ElasticBulkOperationService
{
    private readonly ElasticsearchClientProvider _elasticsearchClientProvider;
    private readonly IndexOptions _indexOptions;
    private readonly ILogger<ElasticBulkOperationService> _logger;

    public ElasticBulkOperationService(ElasticsearchClientProvider elasticsearchClientProvider,
        IOptions<ElasticsearchClientOptions> elasticsearchClientOptions, ILogger<ElasticBulkOperationService> logger)
    {
        _elasticsearchClientProvider = elasticsearchClientProvider;
        _indexOptions = elasticsearchClientOptions.Value.IndexOptions;
        _logger = logger;
    }

    public async Task BulkUpsertAsync(IEnumerable<ElasticDocument> documents)
    {
        if (!documents.Any())
        {
            return;
        }

        var client = await _elasticsearchClientProvider.Client;
        var upsertBulkRequestDescriptorFactory = new UpsertBulkRequestDescriptorFactory(documents);
        var bulkUpsertRequestDescriptor = upsertBulkRequestDescriptorFactory.Create();

        var bulkUpsertResponse = await client.BulkAsync(bulkUpsertRequestDescriptor);
        if (bulkUpsertResponse.IsValidResponse)
        {
            return;
        }

        string reason = bulkUpsertResponse.GetExceptionMessage();
        var bulkIndexException = new BulkIndexException(reason);
        _logger.LogError(bulkIndexException, "Could not bulk insert documents.");
        throw bulkIndexException;
    }

    public async Task BulkDeleteAsync(IEnumerable<ElasticDocument> documents)
    {
        if (!documents.Any())
        {
            return;
        }

        var client = await _elasticsearchClientProvider.Client;
        var deleteBulkRequestDescriptorFactory = new DeleteBulkRequestDescriptorFactory(_indexOptions.IndexName, documents);
        var bulkDeleteRequestDescriptor = deleteBulkRequestDescriptorFactory.Create();

        var bulkDeleteResponse = await client.BulkAsync(bulkDeleteRequestDescriptor);
        if (bulkDeleteResponse.IsValidResponse)
        {
            return;
        }

        string reason = bulkDeleteResponse.GetExceptionMessage();
        var bulkDeleteException = new BulkDeleteException(reason);
        _logger.LogError(bulkDeleteException, "Failed to delete documents.");
        throw bulkDeleteException;
    }

    public async Task PurgeTableAsync(ElasticTablePurgeDto elasticTablePurgeDto)
    {
        var client = await _elasticsearchClientProvider.Client;
        var deleteByRequestDescriptorFactory = new DeleteByRequestDescriptorFactory(_indexOptions.IndexName, elasticTablePurgeDto);
        var tablePurgeRequestDescriptor = deleteByRequestDescriptorFactory.Create();

        var deleteByQueryResponse = await client.DeleteByQueryAsync(tablePurgeRequestDescriptor);
        if (deleteByQueryResponse.IsValidResponse)
        {
            return;
        }

        string reason = deleteByQueryResponse.GetExceptionMessage();
        var deleteByQueryException = new DeleteByQueryException(reason);
        _logger.LogError(deleteByQueryException, "Failed to delete documents.");
        throw deleteByQueryException;
    }
}
