using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Synchronization;

using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Factories;
internal class UpsertBulkRequestDescriptorFactory : IFactory<BulkRequestDescriptor>
{
    private readonly IEnumerable<ElasticDocument> _documents;

    public UpsertBulkRequestDescriptorFactory(IEnumerable<ElasticDocument> documents)
    {
        _documents = documents;
    }

    public BulkRequestDescriptor Create()
    {
        var bulkRequestDescriptor = new BulkRequestDescriptor();
        bulkRequestDescriptor.IndexMany(_documents, 
            (descriptor, document) => descriptor.Id(CreateId(document)));

        return bulkRequestDescriptor;
    }

    // TODO REFACTOR DUPLICATION IN REQUEST DESCRIPTOR FACTORIES
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
