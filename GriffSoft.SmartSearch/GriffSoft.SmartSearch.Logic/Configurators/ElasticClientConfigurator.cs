using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

using GriffSoft.SmartSearch.Logic.Settings;

using System;
using System.Diagnostics;
using System.Text;

namespace GriffSoft.SmartSearch.Logic.Configurators;
internal class ElasticClientConfigurator
{
    private readonly ElasticClientSettings _elasticClientSettings;

    public ElasticClientConfigurator(ElasticClientSettings elasticClientSettings)
    {
        _elasticClientSettings = elasticClientSettings;
    }

    public ElasticsearchClient CreateClient()
    {
        var elasticClientSettings = CreateClientSettings();
        return new ElasticsearchClient(elasticClientSettings);
    }

    private ElasticsearchClientSettings CreateClientSettings()
    {
        var settings = new ElasticsearchClientSettings(new Uri(_elasticClientSettings.Url))
            .DefaultFieldNameInferrer(f => f)
            .CertificateFingerprint(_elasticClientSettings.Fingerprint)
            .Authentication(new BasicAuthentication(_elasticClientSettings.Username, _elasticClientSettings.Password));

        return settings;
    }
}
