using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;

using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.Factories;
public class DeleteBulkRequestDescriptorFactory : IFactory<BulkRequestDescriptor>
{
    private readonly string _indexName;
    private readonly IEnumerable<ElasticDocument> _documents;

    public DeleteBulkRequestDescriptorFactory(string indexName, IEnumerable<ElasticDocument> documents)
    {
        _indexName = indexName;
        _documents = documents;
    }

    public BulkRequestDescriptor Create()
    {
        var ids = _documents.Select(CreateId);
        var bulkRequestDescriptor = new BulkRequestDescriptor();
        bulkRequestDescriptor.DeleteMany(_indexName, ids);

        return bulkRequestDescriptor;
    }

    private Id CreateId(ElasticDocument document)
    {
        var elasticDocumentIdHashDto = new ElasticDocumentIdHashDto
        {
            Server = document.Server,
            Database = document.Database,
            Table = document.Table,
            Type = document.Type,
            Keys = document.Keys,
            Column = document.Column,
        };

        var idHashFactory = new IdHashFactory(elasticDocumentIdHashDto);
        return idHashFactory.Create();
    }
}
