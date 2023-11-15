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

namespace GriffSoft.SmartSearch.Logic.Services;
/// <summary>
/// TODO
/// </summary>
/// <remarks>Has to be singleton</remarks>
public class ElasticsearchService : ISearchService
{
    private readonly ElasticsearchClient _elasticsearchClient;
    private readonly IndexSettings _indexSettings;
    private readonly DataInitializerSettings _dataInitializerSettings;
    private readonly ElasticClientSettings _elasticClientSettings;

    public ElasticsearchService(ElasticClientSettings elasticClientSettings, IndexSettings indexSettings,
        DataInitializerSettings dataInitializerSettings)
    {
        _elasticClientSettings = elasticClientSettings;
        _elasticsearchClient = new ElasticsearchClient(_elasticClientSettings.Settings);
        _indexSettings = indexSettings;
        _dataInitializerSettings = dataInitializerSettings;
    }

    public async Task InitializeDataAsync()
    {
        await CreateIndexIfAbsentAsync();

        foreach (var elasticTarget in _dataInitializerSettings.ElasticTargets)
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
                        BatchSize = _dataInitializerSettings.BatchSize,
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
        var existsResponse = await _elasticsearchClient.Indices.ExistsAsync(_indexSettings.IndexName);
        if (!existsResponse.Exists)
        {
            await CreateIndexAsync();
        }
    }
        
    private async Task CreateIndexAsync()
    {
        var indexResponse = await _elasticsearchClient
            .Indices.CreateAsync(_indexSettings.IndexDescriptor);

        if (!indexResponse.ShardsAcknowledged)
        {
            // TODO SPECIFIC EXCEPTION
            throw new Exception($"Error occurred while trying to create {_indexSettings.IndexName}.");
        }

        // TODO LOG
        Console.WriteLine($"A {_indexSettings.IndexName} nevu index sikeresen letrehozva.");
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
            var bulkResponse = await _elasticsearchClient.IndexManyAsync(documents, _indexSettings.IndexName);
            if (!bulkResponse.IsValidResponse)
            {
                // TODO GET THE ERROR MESSAGE
                throw new Exception($"Error occurred while adding documents to {_indexSettings.IndexName}.");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    public async Task<SearchResult<T>> SearchAsync<T>(PaginatedSearchQuery paginatedSearchQuery) where T : class
    {
        var searchResponse = await _elasticsearchClient.SearchAsync<T>(rd => rd
            .Index(_indexSettings.IndexName)
            .Query(qd => qd.MultiMatch(mqd => mqd
                .Type(TextQueryType.BoolPrefix)
                .Fields(new[] { "value", "value._2gram", "value._3gram" })
                .Query(paginatedSearchQuery.Query)))
            .Size(paginatedSearchQuery.Size)
            .From(paginatedSearchQuery.Offset));

        if (!searchResponse.IsValidResponse)
        {
            throw new Exception("An error occurred trying to perform search.");
        }

        return searchResponse.ToSearchResult();
    }
}
