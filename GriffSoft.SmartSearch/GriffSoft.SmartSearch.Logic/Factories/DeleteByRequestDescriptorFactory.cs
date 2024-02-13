using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Synchronization;

namespace GriffSoft.SmartSearch.Logic.Factories;
internal class DeleteByRequestDescriptorFactory : IFactory<DeleteByQueryRequestDescriptor<ElasticDocument>>
{
    private readonly string _indexName;
    private readonly ElasticTablePurgeDto _elasticTablePurgeDto;

    public DeleteByRequestDescriptorFactory(string indeName, ElasticTablePurgeDto elasticTablePurgeDto)
    {
        _indexName = indeName;
        _elasticTablePurgeDto = elasticTablePurgeDto;
    }

    public DeleteByQueryRequestDescriptor<ElasticDocument> Create()
    {
        var deleteByQueryRequestDescriptor = new DeleteByQueryRequestDescriptor<ElasticDocument>(_indexName);
        deleteByQueryRequestDescriptor.Query(q => q
            .Bool(b => b.Must(
                m => m.Term(t => t.Server, _elasticTablePurgeDto.Server),
                m => m.Term(t => t.Database, _elasticTablePurgeDto.Database),
                m => m.Term(t => t.Table, _elasticTablePurgeDto.Table))));

        return deleteByQueryRequestDescriptor;
    }
}
