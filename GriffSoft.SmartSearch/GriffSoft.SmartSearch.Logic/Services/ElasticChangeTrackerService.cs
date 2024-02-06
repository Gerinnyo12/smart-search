using GriffSoft.SmartSearch.Database.Connection;
using GriffSoft.SmartSearch.Database.DataRead;
using GriffSoft.SmartSearch.Database.Dtos;
using GriffSoft.SmartSearch.Database.Factories;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Exceptions;
using GriffSoft.SmartSearch.Logic.Extensions;
using GriffSoft.SmartSearch.Logic.Factories;
using GriffSoft.SmartSearch.Logic.Mapping;
using GriffSoft.SmartSearch.Logic.Options;
using GriffSoft.SmartSearch.Logic.Providers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services;
public class ElasticChangeTrackerService : IChangeTrackerService<ElasticDocument>
{
    private readonly ElasticsearchClientProvider _elasticsearchClientProvider;
    private readonly ElasticsearchData _elasticsearchData;
    private readonly IndexOptions _indexOptions;
    private readonly ILogger<ElasticChangeTrackerService> _logger;

    public DateTime LastSynchonizationDate { get; private set; } = DateTime.MinValue;

    public ElasticChangeTrackerService(ElasticsearchClientProvider elasticsearchClientProvider, IOptions<ElasticsearchData> elasticsearchData, 
        IOptions<IndexOptions> indexOptions, ILogger<ElasticChangeTrackerService> logger)
    {
        _elasticsearchClientProvider = elasticsearchClientProvider;
        _elasticsearchData = elasticsearchData.Value;
        _indexOptions = indexOptions.Value;
        _logger = logger;
    }

    public async Task TrackChangesAsync()
    {
        DateTime syncDate = DateTime.UtcNow;

        foreach (var elasticTarget in _elasticsearchData.ElasticTargets)
        {
            using var sqlConnector = new SqlConnector(elasticTarget.ConnectionString);

            foreach (var elasticTable in elasticTarget.Tables)
            {
                // TODO MAPPING
                var sqlBatchUpsertQueryDto = new SqlBatchUpsertQueryDto
                {
                    SqlConnector = sqlConnector,
                    LastSyncDate = LastSynchonizationDate,
                    CurrentSyncDate = syncDate,
                    Table = elasticTable.Table,
                    Keys = elasticTable.Keys,
                    Columns = elasticTable.Columns,
                    BatchSize = _elasticsearchData.BatchSize,
                };
                var sqlBatchUpsertQueryFactory = new SqlBatchUpsertQueryFactory(sqlBatchUpsertQueryDto);
                var sqlBatchDeleteQueryDto = new SqlBatchDeleteQueryDto
                {
                    SqlConnector = sqlConnector,
                    LastSyncDate = LastSynchonizationDate,
                    CurrentSyncDate = syncDate,
                    Table = elasticTable.Table,
                    Keys = elasticTable.Keys,
                    Columns = elasticTable.Columns,
                    BatchSize = _elasticsearchData.BatchSize,
                };
                var sqlBatchDeleteQueryFactory = new SqlBatchDeleteQueryFactory(sqlBatchDeleteQueryDto);
                var elasticDocumentMapperDto = new ElasticDocumentMapperDto
                {
                    Server = elasticTarget.Server,
                    Database = elasticTarget.Database,
                    Table = elasticTable.Table,
                    Type = elasticTable.Type,
                    Keys = elasticTable.Keys,
                    Columns = elasticTable.Columns,
                };
                var elasticDocumentMapper = new ElasticDocumentMapper(elasticDocumentMapperDto);

                var sqlBatchUpsertDataReader = new SqlBatchDataReader<ElasticDocument>(sqlBatchUpsertQueryFactory, elasticDocumentMapper);
                await sqlBatchUpsertDataReader.ProcessDataAsync(postProcessCallback: BulkUpsertAsync);

                var sqlBatchDeleteDataReader = new SqlBatchDataReader<ElasticDocument>(sqlBatchDeleteQueryFactory, elasticDocumentMapper);
                await sqlBatchDeleteDataReader.ProcessDataAsync(postProcessCallback: BulkDeleteAsync);
            }
        }

        LastSynchonizationDate = syncDate;
        _logger.LogInformation("Data synchronization successfully ended.");
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

    private async Task BulkDeleteAsync(IEnumerable<ElasticDocument> documents)
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
}
