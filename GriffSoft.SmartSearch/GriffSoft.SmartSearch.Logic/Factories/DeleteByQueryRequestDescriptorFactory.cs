using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;

using System.Collections.Generic;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.Factories;
internal class DeleteByQueryRequestDescriptorFactory : IFactory<DeleteByQueryRequestDescriptor<ElasticDocument>>
{
    private readonly string _indexName;
    private readonly IEnumerable<ElasticDocument> _documents;

    public DeleteByQueryRequestDescriptorFactory(string indexName, IEnumerable<ElasticDocument> documents)
    {
        _indexName = indexName;
        _documents = documents;
    }

    public DeleteByQueryRequestDescriptor<ElasticDocument> Create()
    {
        var deleteByQueryRequestDescriptor = new DeleteByQueryRequestDescriptor<ElasticDocument>(_indexName);
        var ids = _documents.Select(CreateId).ToArray();
        deleteByQueryRequestDescriptor.Query(q => q.Ids(i => i.Values(ids)));

        return deleteByQueryRequestDescriptor;
    }

    private string CreateId(ElasticDocument document)
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
