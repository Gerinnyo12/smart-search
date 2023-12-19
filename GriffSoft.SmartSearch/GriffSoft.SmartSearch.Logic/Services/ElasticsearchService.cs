using Elastic.Clients.Elasticsearch;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using GriffSoft.SmartSearch.Logic.Database;
using GriffSoft.SmartSearch.Logic.Mappers;
using GriffSoft.SmartSearch.Logic.Extensions;
using GriffSoft.SmartSearch.Logic.Configurators;
using SearchRequest = GriffSoft.SmartSearch.Logic.Dtos.SearchRequest;
using GriffSoft.SmartSearch.Logic.Appliers;

namespace GriffSoft.SmartSearch.Logic.Services;
/// <summary>
/// TODO
/// </summary>
/// <remarks>Has to be singleton</remarks>
public class ElasticSearchService : ISearchService<ElasticDocument>
{
    private readonly ElasticIndexConfigurator _elasticIndexConfigurator;
    private readonly ElasticsearchData _elasticsearchData;
    private readonly ElasticsearchClient _elasticsearchClient;

    public ElasticSearchService(IndexSettings indexSettings,
        ElasticsearchData elasticsearchData, ElasticClientSettings elasticClientSettings)
    {
        InvalidateIncorrectSettings(indexSettings);
        InvalidateIncorrectSettings(elasticClientSettings);
        InvalidateIncorrectSettings(elasticsearchData);

        _elasticIndexConfigurator = new ElasticIndexConfigurator(indexSettings);
        _elasticsearchData = elasticsearchData;
        var elasticClientConfigurator = new ElasticClientConfigurator(elasticClientSettings);
        _elasticsearchClient = elasticClientConfigurator.CreateClient();
    }

    private void InvalidateIncorrectSettings(IValidatable settings) => settings.InvalidateIfIncorrect();

    public async Task EnsureAvailableAsync()
    {
        var pingResponse = await _elasticsearchClient.PingAsync();
        if (!pingResponse.IsValidResponse)
        {
            string exceptionMessage = pingResponse.ApiCallDetails.OriginalException?.Message ?? "Unknown";
            throw new Exception($"Elastic client is not working. The reason is: {exceptionMessage}");
        }
    }

    private async Task SafeIndexManyAsync(IEnumerable<ElasticDocument> documents)
    {
        if (!documents.Any())
        {
            return;
        }

        // TODO
        try
        {
            var bulkRequestDescriptor = _elasticIndexConfigurator.CreateBulkUpsertDescriptor(documents);
            var bulkResponse = await _elasticsearchClient.BulkAsync(bulkRequestDescriptor);
            if (!bulkResponse.IsValidResponse)
            {
                // TODO GET THE ERROR MESSAGE
                throw new Exception($"Error occurred while adding documents to {_elasticIndexConfigurator.IndexName}.");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private async Task CreateIndexAsync()
    {
        var indexResponse = await _elasticsearchClient
            .Indices.CreateAsync(_elasticIndexConfigurator.IndexDescriptor);

        if (!indexResponse.ShardsAcknowledged)
        {
            // TODO SPECIFIC EXCEPTION
            throw new Exception($"Error occurred while trying to create {_elasticIndexConfigurator.IndexName}.");
        }

        // TODO LOG
        Console.WriteLine($"A {_elasticIndexConfigurator.IndexName} nevu index sikeresen letrehozva.");
    }

    private async Task CreateIndexIfAbsentAsync()
    {
        var existsResponse = await _elasticsearchClient.Indices.ExistsAsync(_elasticIndexConfigurator.IndexName);
        if (!existsResponse.Exists)
        {
            await CreateIndexAsync();
        }
    }

    public async Task PrepareDataAsync()
    {
        await CreateIndexIfAbsentAsync();

        foreach (var elasticTarget in _elasticsearchData.ElasticTargets)
        {
            using var sqlConnector = new SqlConnector(elasticTarget.ConnectionString);

            foreach (var elasticTable in elasticTarget.Tables)
            {
                foreach (var column in elasticTable.Columns)
                {
                    var elasticQueryProperties = new ElasticQueryProperties
                    {
                        Column = column,
                        Keys = elasticTable.Keys,
                        Table = elasticTable.Table,
                        BatchSize = _elasticsearchData.BatchSize,
                    };
                    var queryBuilder = new ElasticQueryBuilder(sqlConnector, elasticQueryProperties);

                    var elasticMapperProperites = new ElasticMapperProperties
                    {
                        Server = elasticTarget.Server,
                        Database = elasticTarget.Database,
                        Table = elasticTable.Table,
                        Type = elasticTable.Type,
                        Column = column,
                        Keys = elasticTable.Keys,
                    };

                    var mapper = new ElasticReaderMapper(elasticMapperProperites);
                    var dataReader = new SqlDataReader(queryBuilder, mapper);
                    await dataReader.ProcessDataInBatchesAsync(SafeIndexManyAsync);
                }
            }
        }
        // TODO LOG
    }

    public async Task<SearchResult<ElasticDocument>> SearchAsync(SearchRequest searchRequest)
    {
        var requestApplier = new RequestApplier(searchRequest);
        var searchRequestDescriptor = new SearchRequestDescriptor<ElasticDocument>();
        searchRequestDescriptor = requestApplier
            .ApplyRequest(searchRequestDescriptor)
            .Index(_elasticIndexConfigurator.IndexName);

        var searchResponse = await _elasticsearchClient.SearchAsync(searchRequestDescriptor);
        if (!searchResponse.IsValidResponse)
        {
            throw new Exception("An error occurred trying to perform search.");
        }

        return searchResponse.ToSearchResult();
    }
}
