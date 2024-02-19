using Elastic.Clients.Elasticsearch.IndexManagement;

namespace GriffSoft.SmartSearch.Logic.Factories;
internal class DropIndexRequestDescriptorFactory : IFactory<DeleteIndexRequestDescriptor>
{
    private readonly string _indexName;

    public DropIndexRequestDescriptorFactory(string indexName)
    {
        _indexName = indexName;
    }

    public DeleteIndexRequestDescriptor Create()
    {
        var dropIndexRequestDescriptor = new DeleteIndexRequestDescriptor(_indexName);
        return dropIndexRequestDescriptor;
    }
}
