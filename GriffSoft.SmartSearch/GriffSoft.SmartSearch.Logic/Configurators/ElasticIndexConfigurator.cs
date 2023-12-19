using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using GriffSoft.SmartSearch.Logic.Dtos;

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using IndexSettings = GriffSoft.SmartSearch.Logic.Settings.IndexSettings;

namespace GriffSoft.SmartSearch.Logic.Configurators;
internal class ElasticIndexConfigurator
{
    private readonly IndexSettings _indexSettings;

    public string IndexName => _indexSettings.IndexName;

    public CreateIndexRequestDescriptor<ElasticDocument> IndexDescriptor => CreateIndexDescriptor();

    public ElasticIndexConfigurator(IndexSettings indexSettings)
    {
        _indexSettings = indexSettings;
    }

    private CreateIndexRequestDescriptor<ElasticDocument> CreateIndexDescriptor()
    {
        var indexDescriptor = new CreateIndexRequestDescriptor<ElasticDocument>(_indexSettings.IndexName);
        indexDescriptor
            .Settings(i => i
            .NumberOfShards(_indexSettings.NumberOfShards)
            .NumberOfReplicas(_indexSettings.NumberOfReplicas))
            .Mappings(t => t.Properties(p => p
                .Keyword(d => d.Server, d => d.Index(false))
                .Keyword(d => d.Database, d => d.Index(false))
                .Keyword(d => d.Table, d => d.Index(false))
                .IntegerNumber(d => d.Type, d => d.Index(false))
                .Keyword(d => d.Column, d => d.Index(false))
                .Object(d => d.Keys, d => d.Enabled(false))
                .SearchAsYouType(d => d.Value)));

        return indexDescriptor;
    }

    public BulkRequestDescriptor CreateBulkUpsertDescriptor(IEnumerable<ElasticDocument> documents)
    {
        var bulkRequestDescriptor = new BulkRequestDescriptor();
        bulkRequestDescriptor.IndexMany(documents, (descriptor, document) => descriptor
            .Id(CreateId(document))
            .Index(_indexSettings.IndexName));

        return bulkRequestDescriptor;
    }

    private string CreateId(ElasticDocument document)
    {
        // TODO MAPPER
        var elasticDocumentIdProperties = new ElasticDocumentIdProperties
        {
            Server = document.Server,
            Database = document.Database,
            Table = document.Table,
            Column = document.Column,
            Keys = document.Keys,
        };

        string hashedId = HashDocument(elasticDocumentIdProperties);
        return hashedId;
    }

    private string HashDocument(ElasticDocumentIdProperties elasticDocumentIdProperties)
    {
        string json = JsonSerializer.Serialize(elasticDocumentIdProperties);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using var hashAlgorithm = MD5.Create();
        var hashedBytes = hashAlgorithm.ComputeHash(jsonBytes);
        string hash = string.Concat(hashedBytes.Select(b => b.ToString("X2")));

        return hash;
    }
}
