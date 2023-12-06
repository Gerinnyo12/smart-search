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
using Elastic.Clients.Elasticsearch.QueryDsl;
using GriffSoft.SmartSearch.Logic.Configurators;

namespace GriffSoft.SmartSearch.Logic.Services;
/// <summary>
/// TODO
/// </summary>
/// <remarks>Has to be singleton</remarks>
public class ElasticsearchService : ISearchService
{
    private readonly IndexConfigurator _indexConfigurator;
    private readonly ElasticsearchData _elasticsearchData;
    private readonly ElasticsearchClient _elasticsearchClient;

    public ElasticsearchService(IndexSettings indexSettings,
        ElasticsearchData elasticsearchData, ElasticClientSettings elasticClientSettings)
    {
        InvalidateIncorrectSettings(indexSettings);
        InvalidateIncorrectSettings(elasticClientSettings);
        InvalidateIncorrectSettings(elasticsearchData);

        _indexConfigurator = new IndexConfigurator(indexSettings);
        _elasticsearchData = elasticsearchData;
        var elasticsearchClientConfigurator = new ElasticsearchClientConfigurator(elasticClientSettings);
        _elasticsearchClient = new ElasticsearchClient(elasticsearchClientConfigurator.ClientSettings);
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
                    var elasticQueryParameters = new ElasticQueryParameters
                    {
                        Column = column,
                        Keys = elasticTable.Keys,
                        Table = elasticTable.Table,
                        BatchSize = _elasticsearchData.BatchSize,
                    };
                    var queryBuilder = new ElasticQueryBuilder(sqlConnector, elasticQueryParameters);

                    var elasticMapperParameters = new ElasticMapperParameters
                    {
                        Server = elasticTarget.Server,
                        Database = elasticTarget.Database,
                        Table = elasticTable.Table,
                        Column = column,
                        Keys = elasticTable.Keys,
                    };
                    var mapper = new ElasticMapper(elasticMapperParameters);

                    var dataReader = new DataReader<ElasticDocument>(queryBuilder, mapper);
                    await dataReader.ProcessDataInBatchesAsync(SafeIndexManyAsync);
                }
            }
        }

        // TODO LOG
    }

    private async Task CreateIndexIfAbsentAsync()
    {
        var existsResponse = await _elasticsearchClient.Indices.ExistsAsync(_indexConfigurator.IndexName);
        if (!existsResponse.Exists)
        {
            await CreateIndexAsync();
        }
    }
        
    private async Task CreateIndexAsync()
    {
        var indexResponse = await _elasticsearchClient
            .Indices.CreateAsync(_indexConfigurator.IndexDescriptor);

        if (!indexResponse.ShardsAcknowledged)
        {
            // TODO SPECIFIC EXCEPTION
            throw new Exception($"Error occurred while trying to create {_indexConfigurator.IndexName}.");
        }

        // TODO LOG
        Console.WriteLine($"A {_indexConfigurator.IndexName} nevu index sikeresen letrehozva.");
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
            var bulkRequestDescriptor = _indexConfigurator.CreateBulkUpsertDescriptor(documents);
            var bulkResponse = await _elasticsearchClient.BulkAsync(bulkRequestDescriptor);
            if (!bulkResponse.IsValidResponse)
            {
                // TODO GET THE ERROR MESSAGE
                throw new Exception($"Error occurred while adding documents to {_indexConfigurator.IndexName}.");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    public async Task<SearchResult<T>> SearchAsync<T>(PaginatedSearchQuery paginatedSearchQuery) where T : class
    {
        if (paginatedSearchQuery is not ElasticsearchQuery<T> elasticsearchQuery)
        {
            throw new ArgumentException($"{nameof(paginatedSearchQuery)} is not an instance of {nameof(ElasticsearchQuery<T>)}");
        }

        var searchResponse = await _elasticsearchClient.SearchAsync<T>(rd => rd
            .Index(_indexConfigurator.IndexName)
            .Query(qd => qd.MultiMatch(mqd => mqd
                .Type(TextQueryType.BoolPrefix)
                .Fields(new[] { "Value", "Value._2gram", "Value._3gram" })
                .Query(elasticsearchQuery.Query)))
            .Sort(elasticsearchQuery.SortDescriptor)
            .Size(elasticsearchQuery.Size)
            .From(elasticsearchQuery.Offset));

        if (!searchResponse.IsValidResponse)
        {
            throw new Exception("An error occurred trying to perform search.");
        }

        return searchResponse.ToSearchResult();
    }
}
