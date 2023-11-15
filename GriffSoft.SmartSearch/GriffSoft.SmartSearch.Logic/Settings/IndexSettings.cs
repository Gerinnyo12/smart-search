using Elastic.Clients.Elasticsearch.IndexManagement;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Interfaces;

using System;

namespace GriffSoft.SmartSearch.Logic.Settings;
public class IndexSettings : IValidatable
{
    public string IndexName { get; init; } = "search-index";

    public int NumberOfShards { get; init; } = 3;

    public int NumberOfReplicas { get; init; } = 0;

    internal CreateIndexRequestDescriptor<ElasticDocument> IndexDescriptor => BuildIndexDescriptor();

    private CreateIndexRequestDescriptor<ElasticDocument> BuildIndexDescriptor()
    {
        var indexDescriptor = new CreateIndexRequestDescriptor<ElasticDocument>(IndexName);
        indexDescriptor
            .Settings(i => i
            .NumberOfShards(NumberOfShards)
            .NumberOfReplicas(NumberOfReplicas))
            .Mappings(t => t.Properties(p => p
                .Keyword(d => d.Server, d => d.Index(false))
                .Keyword(d => d.Database, d => d.Index(false))
                .Keyword(d => d.Table, d => d.Index(false))
                .Keyword(d => d.Column, d => d.Index(false))
                .Object(d => d.Keys, d => d.Enabled(false))
                .SearchAsYouType(d => d.Value)));

        return indexDescriptor;
    }

    public void InvalidateIfIncorrect()
    {
        if (string.IsNullOrWhiteSpace(IndexName))
        {
            throw new Exception($"{nameof(IndexName)} must be provided.");
        }

        if (NumberOfShards <= 0)
        {
            throw new Exception($"{nameof(NumberOfShards)} must be provided and must not be negative.");
        }

        if (NumberOfReplicas < 0)
        {
            throw new Exception($"{nameof(NumberOfReplicas)} must be provided and must not be negative.");
        }
    }
}
