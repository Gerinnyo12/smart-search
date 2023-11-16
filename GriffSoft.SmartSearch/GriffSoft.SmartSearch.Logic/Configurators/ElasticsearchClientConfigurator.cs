using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

using GriffSoft.SmartSearch.Logic.Settings;

using System;

namespace GriffSoft.SmartSearch.Logic.Configurators;
internal class ElasticsearchClientConfigurator
{
    private readonly ElasticClientSettings _elasticClientSettings;

    public ElasticsearchClientSettings ClientSettings => CreateClientSettings();

    public ElasticsearchClientConfigurator(ElasticClientSettings elasticClientSettings)
    {
        _elasticClientSettings = elasticClientSettings;
    }

    private ElasticsearchClientSettings CreateClientSettings()
    {
        var settings = new ElasticsearchClientSettings(new Uri(_elasticClientSettings.Url))
            .CertificateFingerprint(_elasticClientSettings.Fingerprint)
            .Authentication(new BasicAuthentication(_elasticClientSettings.Username, _elasticClientSettings.Password));

        return settings;
    }
}
