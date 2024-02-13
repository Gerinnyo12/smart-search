using GriffSoft.SmartSearch.Database.DataRead;
using GriffSoft.SmartSearch.Database.Dtos;
using GriffSoft.SmartSearch.Database.Factories;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Synchronization;
using GriffSoft.SmartSearch.Logic.Mapping;
using GriffSoft.SmartSearch.Logic.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services.Synchronization;
public class ElasticChangeTrackerService : ElasticSynchronizerService
{
    private readonly ElasticBulkOperationService _elasticBulkOperationService;

    public ElasticChangeTrackerService(ElasticBulkOperationService elasticBulkOperationService,
        IOptions<ElasticsearchData> elasticsearchData, ILogger<ElasticSynchronizerService> logger) : base(elasticsearchData, logger)
    {
        _elasticBulkOperationService = elasticBulkOperationService;
    }

    protected override async Task RunSynchronizationAsync(ElasticSynchronizationDto elasticSynchronizationDto)
    {
        var sqlBatchUpsertQueryFactory = CreateUpsertQueryFactory(elasticSynchronizationDto);
        var sqlBatchDeleteQueryFactory = CreateDeleteQueryFactory(elasticSynchronizationDto);
        var elasticDocumentMapper = CreateMapper(elasticSynchronizationDto);

        var sqlBatchUpsertDataReader = new SqlBatchDataReader<ElasticDocument>(sqlBatchUpsertQueryFactory, elasticDocumentMapper);
        await sqlBatchUpsertDataReader.ProcessDataAsync(postProcessCallback: _elasticBulkOperationService.BulkUpsertAsync);

        var sqlBatchDeleteDataReader = new SqlBatchDataReader<ElasticDocument>(sqlBatchDeleteQueryFactory, elasticDocumentMapper);
        await sqlBatchDeleteDataReader.ProcessDataAsync(postProcessCallback: _elasticBulkOperationService.BulkDeleteAsync);
    }

    private SqlBatchUpsertQueryFactory CreateUpsertQueryFactory(ElasticSynchronizationDto elasticSynchronizationDto)
    {
        var sqlBatchUpsertQueryDto = new SqlBatchUpsertQueryDto
        {
            SqlConnector = elasticSynchronizationDto.SqlConnector,
            LastSyncDate = elasticSynchronizationDto.LastSynchronizationDate,
            CurrentSyncDate = elasticSynchronizationDto.SynchronizationDate,
            Table = elasticSynchronizationDto.Table.Table,
            Keys = elasticSynchronizationDto.Table.Keys,
            Columns = elasticSynchronizationDto.Table.Columns,
            BatchSize = elasticSynchronizationDto.BatchSize,
        };

        return new SqlBatchUpsertQueryFactory(sqlBatchUpsertQueryDto);
    }

    private SqlBatchDeleteQueryFactory CreateDeleteQueryFactory(ElasticSynchronizationDto elasticSynchronizationDto)
    {
        var sqlBatchDeleteQueryDto = new SqlBatchDeleteQueryDto
        {
            SqlConnector = elasticSynchronizationDto.SqlConnector,
            LastSyncDate = elasticSynchronizationDto.LastSynchronizationDate,
            CurrentSyncDate = elasticSynchronizationDto.SynchronizationDate,
            Table = elasticSynchronizationDto.Table.Table,
            Keys = elasticSynchronizationDto.Table.Keys,
            Columns = elasticSynchronizationDto.Table.Columns,
            BatchSize = elasticSynchronizationDto.BatchSize,
        };

        return new SqlBatchDeleteQueryFactory(sqlBatchDeleteQueryDto);
    }

    private ElasticDocumentMapper CreateMapper(ElasticSynchronizationDto elasticSynchronizationDto)
    {
        var elasticDocumentMapperDto = new ElasticDocumentMapperDto
        {
            Server = elasticSynchronizationDto.Target.Server,
            Database = elasticSynchronizationDto.Target.Database,
            Table = elasticSynchronizationDto.Table.Table,
            Type = elasticSynchronizationDto.Table.Type,
            Keys = elasticSynchronizationDto.Table.Keys,
            Columns = elasticSynchronizationDto.Table.Columns,
        };

        return new ElasticDocumentMapper(elasticDocumentMapperDto);
    }
}
