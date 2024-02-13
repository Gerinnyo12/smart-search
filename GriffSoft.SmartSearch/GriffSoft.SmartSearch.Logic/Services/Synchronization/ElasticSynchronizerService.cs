using GriffSoft.SmartSearch.Database.Connection;
using GriffSoft.SmartSearch.Logic.Dtos.Synchronization;
using GriffSoft.SmartSearch.Logic.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services.Synchronization;
public abstract class ElasticSynchronizerService : ISynchronizerService
{
    private readonly ElasticsearchData _elasticsearchData;
    private readonly ILogger<ElasticSynchronizerService> _logger;

    public DateTime LastSynchonizationDate { get; private set; } = DateTime.MinValue;

    public ElasticSynchronizerService(IOptions<ElasticsearchData> elasticsearchData, ILogger<ElasticSynchronizerService> logger)
    {
        _elasticsearchData = elasticsearchData.Value;
        _logger = logger;
    }

    public async Task SynchronizeAsync()
    {
        DateTime synchronizationDate = DateTime.UtcNow;
        _logger.LogInformation("Synchronization started");

        foreach (var elasticTarget in _elasticsearchData.ElasticTargets)
        {
            try
            {
                using var sqlConnector = new SqlConnector(elasticTarget.ConnectionString);

                foreach (var elasticTable in elasticTarget.Tables)
                {
                    var elasticSyncrhonizationDto = new ElasticSynchronizationDto
                    {
                        Table = elasticTable,
                        SqlConnector = sqlConnector,
                        Target = elasticTarget,
                        LastSynchronizationDate = LastSynchonizationDate,
                        SynchronizationDate = synchronizationDate,
                        BatchSize = _elasticsearchData.BatchSize,
                    };

                    try
                    {
                        await RunSynchronizationAsync(elasticSyncrhonizationDto);
                        _logger.LogInformation("Server: '{server}', Database: '{database}', Table: '{table}' has successfully been synchronized.",
                            elasticTarget.Server, elasticTarget.Database, elasticTable.Table);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "An unhandled error occured during the synchronization of " +
                            "Server: '{server}', Database: '{database}', Table: '{table}'.", elasticTarget.Server, elasticTarget.Database, elasticTable.Table);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not connect to '{server}', '{database}' with connection string '{connectionString}'.",
                    elasticTarget.Server, elasticTarget.Database, elasticTarget.ConnectionString);
            }
        }

        LastSynchonizationDate = synchronizationDate;
        _logger.LogInformation("Synchronization finished.");
    }

    protected abstract Task RunSynchronizationAsync(ElasticSynchronizationDto elasticSynchronizationDto);
}
