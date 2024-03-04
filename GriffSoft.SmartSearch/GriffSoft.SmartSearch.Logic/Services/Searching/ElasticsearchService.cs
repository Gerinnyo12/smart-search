using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.Exceptions;
using GriffSoft.SmartSearch.Logic.Extensions;
using GriffSoft.SmartSearch.Logic.Providers;
using GriffSoft.SmartSearch.Logic.RequestApplication;

using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

using SearchRequest = GriffSoft.SmartSearch.Logic.Dtos.Searching.SearchRequest;

namespace GriffSoft.SmartSearch.Logic.Services.Searching;
/// <summary>
/// TODO
/// </summary>
/// <remarks>Has to be singleton</remarks>
public class ElasticSearchService : ISearchService<ElasticDocument>
{
    private readonly ElasticsearchClientProvider _elasticsearchClientProvider;
    private readonly ILogger<ElasticSearchService> _logger;

    public ElasticSearchService(ElasticsearchClientProvider elasticsearchClientProvider, ILogger<ElasticSearchService> logger)
    {
        _elasticsearchClientProvider = elasticsearchClientProvider;
        _logger = logger;
    }

    public async Task<SearchResult<ElasticDocument>> SearchAsync(SearchRequest searchRequest)
    {
        var elasticSearchClient = await _elasticsearchClientProvider.Client;
        var requestApplicator = new RequestApplicator(searchRequest);

        var searchResponse = await elasticSearchClient.SearchAsync<ElasticDocument>(requestApplicator.ApplyRequestOn);
        if (!searchResponse.IsValidResponse)
        {
            string reason = searchResponse.GetExceptionMessage();
            var searchRequestException = new SearchRequestException(reason);
            _logger.LogError(searchRequestException, "Failed to execute search request.");
            throw searchRequestException;
        }

        return searchResponse.ToSearchResult();
    }
}
