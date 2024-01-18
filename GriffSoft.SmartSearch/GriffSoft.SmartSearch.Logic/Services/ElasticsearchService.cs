using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Database.Connection.Sql;
using GriffSoft.SmartSearch.Database.DataProcess.Sql;
using GriffSoft.SmartSearch.Database.Dtos;
using GriffSoft.SmartSearch.Database.Factories.Sql;
using GriffSoft.SmartSearch.Logic.Appliers;
using GriffSoft.SmartSearch.Logic.Configurators;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Exceptions;
using GriffSoft.SmartSearch.Logic.Extensions;
using GriffSoft.SmartSearch.Logic.Mapping;
using GriffSoft.SmartSearch.Logic.Settings;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SearchRequest = GriffSoft.SmartSearch.Logic.Dtos.SearchRequest;

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
    private readonly ILogger<ElasticSearchService> _logger;
    private readonly Task _dataPreparationTask;

    public ElasticSearchService(IOptions<IndexSettings> indexSettings, IOptions<ElasticsearchData> elasticsearchData,
        IOptions<ElasticClientSettings> elasticClientSettings, ILogger<ElasticSearchService> logger)
    {

        InvalidateIncorrectSettings(indexSettings.Value);
        InvalidateIncorrectSettings(elasticClientSettings.Value);
        InvalidateIncorrectSettings(elasticsearchData.Value);

        _elasticIndexConfigurator = new ElasticIndexConfigurator(indexSettings.Value);
        _elasticsearchData = elasticsearchData.Value;
        var elasticClientConfigurator = new ElasticClientConfigurator(elasticClientSettings.Value);
        _elasticsearchClient = elasticClientConfigurator.CreateClient();
        _logger = logger;

        _dataPreparationTask = RunDataPreparationAsync();
    }

    private void InvalidateIncorrectSettings(IValidatable settings) => settings.InvalidateIfIncorrect();

    public async Task EnsureAvailableAsync()
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

    public Task PrepareDataAsync() => _dataPreparationTask;

    private async Task RunDataPreparationAsync()
    {
        await CreateIndexIfAbsentAsync();

        foreach (var elasticTarget in _elasticsearchData.ElasticTargets)
        {
            using var sqlConnector = new SqlConnector(elasticTarget.ConnectionString);

            foreach (var elasticTable in elasticTarget.Tables)
            {
                foreach (var column in elasticTable.Columns)
                {
                    var sqlQueryDto = new SqlQueryDto
                    {
                        Column = column,
                        Keys = elasticTable.Keys,
                        Table = elasticTable.Table,
                    };
                    var sqlQueryFactory = new SqlQueryFactory(sqlConnector, sqlQueryDto);

                    var dataReaderDto = new DataReaderDto
                    {
                        Server = elasticTarget.Server,
                        Database = elasticTarget.Database,
                        Table = elasticTable.Table,
                        Type = elasticTable.Type,
                        Column = column,
                        Keys = elasticTable.Keys,
                    };
                    var elasticDocumentDataReaderMapper = new ElasticDocumentDataReaderMapper(dataReaderDto);

                    var sqlDataReader = new SqlDataReader<ElasticDocument>(sqlQueryFactory, elasticDocumentDataReaderMapper);
                    await sqlDataReader.ProcessDataInBatchesAsync(_elasticsearchData.BatchSize, SafeIndexManyAsync);

                    _logger.LogInformation("Database: \"{database}\", Table: \"{table}\", Column: \"{column}\" successfully processed.",
                        elasticTarget.Database, elasticTable.Table, column);
                }
            }
        }

        _logger.LogInformation("Data preparation successfully ended.");
    }

    private async Task CreateIndexIfAbsentAsync()
    {
        var existsResponse = await _elasticsearchClient.Indices.ExistsAsync(_elasticIndexConfigurator.IndexName);
        if (existsResponse.Exists)
        {
            _logger.LogInformation("The {indexName} index already exists.", _elasticIndexConfigurator.IndexName);
            return;
        }

        var indexResponse = await _elasticsearchClient.Indices.CreateAsync(_elasticIndexConfigurator.IndexDescriptor);
        if (indexResponse.ShardsAcknowledged)
        {
            _logger.LogInformation("The {indexName} index was successfully created.", _elasticIndexConfigurator.IndexName);
            return;
        }

        string reason = indexResponse.GetExceptionMessage();
        var indexCreationException = new IndexCreationException(reason);
        _logger.LogError(indexCreationException, "Could not create index.");
        throw indexCreationException;
    }

    private async Task SafeIndexManyAsync(IEnumerable<ElasticDocument> documents)
    {
        if (!documents.Any())
        {
            return;
        }

        var bulkRequestDescriptor = _elasticIndexConfigurator.CreateBulkUpsertDescriptor(documents);
        var bulkResponse = await _elasticsearchClient.BulkAsync(bulkRequestDescriptor);
        if (bulkResponse.IsValidResponse)
        {
            return;
        }

        string reason = bulkResponse.GetExceptionMessage();
        var bulkIndexException = new BulkIndexException(reason);
        _logger.LogError(bulkIndexException, "Could not bulk insert documents.");
        throw bulkIndexException;
    }

    public async Task<SearchResult<ElasticDocument>> SearchAsync(SearchRequest searchRequest)
    {
        await _dataPreparationTask;

        var requestApplier = new RequestApplier(searchRequest);
        var searchRequestDescriptor = new SearchRequestDescriptor<ElasticDocument>();

        searchRequestDescriptor = requestApplier
            .ApplyRequest(searchRequestDescriptor)
            .Index(_elasticIndexConfigurator.IndexName);

        var searchResponse = await _elasticsearchClient.SearchAsync(searchRequestDescriptor);
        if (!searchResponse.IsValidResponse)
        {
            string reason = searchResponse.GetExceptionMessage();
            var searchRequestException = new SearchRequestException(reason);
            _logger.LogError(searchRequestException, "Failed to execute search request.");
            throw searchRequestException;
        }

        return searchResponse.ToSearchResult();
    }
}
