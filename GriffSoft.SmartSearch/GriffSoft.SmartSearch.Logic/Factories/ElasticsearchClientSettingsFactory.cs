using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

using GriffSoft.SmartSearch.Logic.Options;

using System;

namespace GriffSoft.SmartSearch.Logic.Factories;
internal class ElasticsearchClientSettingsFactory : IFactory<ElasticsearchClientSettings>
{
    private readonly ElasticsearchClientOptions _elasticsearchClientOptions;

    public ElasticsearchClientSettingsFactory(ElasticsearchClientOptions elasticsearchClientOptions)
    {
        _elasticsearchClientOptions = elasticsearchClientOptions;
    }

    public ElasticsearchClientSettings Create()
    {
        var settings = new ElasticsearchClientSettings(new Uri(_elasticsearchClientOptions.Url))
            .DefaultIndex(_elasticsearchClientOptions.IndexOptions.IndexName)
            .DefaultFieldNameInferrer(f => f)
            .CertificateFingerprint(_elasticsearchClientOptions.Fingerprint)
            .Authentication(new BasicAuthentication(_elasticsearchClientOptions.Username, _elasticsearchClientOptions.Password));

        return settings;
    }
}
