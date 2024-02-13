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
            .Settings(i => i
                .NumberOfShards(_indexOptions.NumberOfShards)
                .NumberOfReplicas(_indexOptions.NumberOfReplicas))
                .Mappings(t => t.Properties(p => p
                    .Keyword(d => d.Server)
                    .Keyword(d => d.Database)
                    .Keyword(d => d.Table)
                    .Keyword(d => d.Type)
                    .Keyword(d => d.Column)
                    .Object(d => d.Keys, c => c.Enabled(false))
                    .SearchAsYouType(d => d.Value)));

        return indexDescriptor;
    }
}
