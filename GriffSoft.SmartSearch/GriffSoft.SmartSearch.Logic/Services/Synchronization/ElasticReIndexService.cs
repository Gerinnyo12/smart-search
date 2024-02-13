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
public class ElasticReIndexService : ElasticSynchronizerService
{
    private readonly ElasticBulkOperationService _elasticBulkOperationService;

    public ElasticReIndexService(ElasticBulkOperationService elasticBulkOperationService,
        IOptions<ElasticsearchData> elasticsearchData, ILogger<ElasticSynchronizerService> logger) : base(elasticsearchData, logger)
    {
        _elasticBulkOperationService = elasticBulkOperationService;
    }

    protected override async Task RunSynchronizationAsync(ElasticSynchronizationDto elasticSynchronizationDto)
    {
        var elasticTablePurgeDto = CreateElasticTablePurgeDto(elasticSynchronizationDto);
        await _elasticBulkOperationService.PurgeTableAsync(elasticTablePurgeDto);

        var sqlBatchFetchQueryFactory = CreateFetchQueryFactory(elasticSynchronizationDto);
        var elasticDocumentMapper = CreateMapper(elasticSynchronizationDto);

        var sqlBatchInsertDataReader = new SqlBatchDataReader<ElasticDocument>(sqlBatchFetchQueryFactory, elasticDocumentMapper);
        await sqlBatchInsertDataReader.ProcessDataAsync(postProcessCallback: _elasticBulkOperationService.BulkUpsertAsync);
    }

    private ElasticTablePurgeDto CreateElasticTablePurgeDto(ElasticSynchronizationDto elasticSynchronizationDto)
    {
        return new ElasticTablePurgeDto
        {
            Server = elasticSynchronizationDto.Target.Server,
            Database = elasticSynchronizationDto.Target.Database,
            Table = elasticSynchronizationDto.Table.Table,
        };
    }

    private SqlBatchFetchQueryFactory CreateFetchQueryFactory(ElasticSynchronizationDto elasticSynchronizationDto)
    {
        var sqlBatcFetchQueryDto = new SqlBatchFetchQueryDto
        {
            SqlConnector = elasticSynchronizationDto.SqlConnector,
            Table = elasticSynchronizationDto.Table.Table,
            Keys = elasticSynchronizationDto.Table.Keys,
            Columns = elasticSynchronizationDto.Table.Columns,
            BatchSize = elasticSynchronizationDto.BatchSize,
        };

        return new SqlBatchFetchQueryFactory(sqlBatcFetchQueryDto);
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
