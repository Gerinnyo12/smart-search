using Elastic.Clients.Elasticsearch.IndexManagement;

using GriffSoft.SmartSearch.Logic.Dtos;

using IndexOptions = GriffSoft.SmartSearch.Logic.Options.IndexOptions;

namespace GriffSoft.SmartSearch.Logic.Factories;
internal class CreateIndexRequestDescriptorFactory : IFactory<CreateIndexRequestDescriptor<ElasticDocument>>
{
    private readonly IndexOptions _indexOptions;

    public CreateIndexRequestDescriptorFactory(IndexOptions indexOptions)
    {
        _indexOptions = indexOptions;
    }

    public CreateIndexRequestDescriptor<ElasticDocument> Create()
    {
        var indexDescriptor = new CreateIndexRequestDescriptor<ElasticDocument>(_indexOptions.IndexName);
        indexDescriptor
            .Settings(s => s
                .NumberOfShards(_indexOptions.NumberOfShards)
                .NumberOfReplicas(_indexOptions.NumberOfReplicas))
                .Mappings(m => m.Properties(p => p
                    .Keyword(k => k.Server)
                    .Keyword(k => k.Database)
                    .Keyword(k => k.Table)
                    .Keyword(k => k.Type)
                    .Keyword(k => k.Column)
                    .Object(k => k.Keys, d => d.Enabled(false))
                    .Text(t => t.Value, d => d
                        .Fields(f => f
                            .Keyword("Raw")))));

        return indexDescriptor;
    }
}
