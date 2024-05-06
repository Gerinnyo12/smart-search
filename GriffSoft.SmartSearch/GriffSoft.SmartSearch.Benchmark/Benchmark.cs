using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using GriffSoft.SmartSearch.Benchmark.DependecyInjection;
using GriffSoft.SmartSearch.Logic.Builders;
using GriffSoft.SmartSearch.Logic.Dtos.Enums;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.Services.Searching;
using GriffSoft.SmartSearch.Logic.Services.Synchronization;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GriffSoft.SmartSearch.Benchmark;
[SimpleJob(RuntimeMoniker.Net80)]
public class Benchmark : IDisposable
{
    private const string FullTextPrefixQuery = 
        "SELECT {0} " +
        "FROM {1} " +
        "WHERE CONTAINS({2}, '\"{3}*\"') " +
        "ORDER BY {4} " +
        "OFFSET {5} ROWS " +
        "FETCH NEXT {6} ROWS ONLY;";

    private const string SqlWildcardQuery = 
        "SELECT {0} " +
        "FROM {1} " +
        "WHERE {2} LIKE '%{3}%' " +
        "ORDER BY {4} " +
        "OFFSET {5} ROWS " +
        "FETCH NEXT {6} ROWS ONLY;";

    // TODO expand with multi word phrases
    private readonly List<string> _searchPhrases = ["house", "life", "look", "best", "day", "sun", "night", "red", "holiday", "car"];
    private readonly Random _random = new();

    private ServiceProvider _container = default!;
    private ElasticSearchService _elasticSearchService = default!;
    private FullTextSearchData _fullTextSearchData = default!;
    private SqlConnection _sqlConnection = default!;

    [GlobalSetup]
    public async Task SetupAsync()
    {
        var containerFactory = new ContainerFactory();
        _container = containerFactory.CreateConatiner();

        var elasticReIndexService = _container.GetRequiredService<ElasticReIndexService>();
        _elasticSearchService = _container.GetRequiredService<ElasticSearchService>();

        await elasticReIndexService.SynchronizeAsync();

        _fullTextSearchData = _container.GetRequiredService<IOptions<FullTextSearchData>>().Value;
        _sqlConnection = new SqlConnection(_fullTextSearchData.ConnectionString);
        await _sqlConnection.OpenAsync();
    }

    [Benchmark]
    public async Task ElasticsearchBoolPrefixAsync()
    {
        var searchRequest = new SearchRequestBuilder()
            .Filters(new SearchFilter
            {
                FieldName = "Value",
                FieldValue = _searchPhrases[_random.Next(_searchPhrases.Count)],
                FilterMatchType = FilterMatchType.BoolPrefix,
            })
            .Sorts(new SearchSort
            {
                FieldName = "Database",
                SortDirection = SortDirection.Ascending,
            })
            .Take(500)
            .Skip(0)
            .Build();

        await _elasticSearchService.SearchAsync(searchRequest);
    }

    [Benchmark]
    public async Task FullTextSearchPrefixAsync()
    {
        using var sqlCommand = new SqlCommand()
        {
            Connection = _sqlConnection,
            CommandText = string.Format(FullTextPrefixQuery, _fullTextSearchData.Column, _fullTextSearchData.Table,
                _fullTextSearchData.Column, _searchPhrases[_random.Next(_searchPhrases.Count)],
                _fullTextSearchData.Id, _fullTextSearchData.Offset, _fullTextSearchData.Size),
        };

        using var reader = await sqlCommand.ExecuteReaderAsync();
        await reader.NextResultAsync();
    }

    [Benchmark]
    public async Task ElasticsearchWildcardAsync()
    {
        var searchRequest = new SearchRequestBuilder()
            .Filters(new SearchFilter
            {
                FieldName = "Value",
                FieldValue = _searchPhrases[_random.Next(_searchPhrases.Count)],
                FilterMatchType = FilterMatchType.Wildcard,
            })
            .Sorts(new SearchSort
            {
                FieldName = "Database",
                SortDirection = SortDirection.Ascending,
            })
            .Take(500)
            .Skip(0)
            .Build();

        await _elasticSearchService.SearchAsync(searchRequest);
    }

    [Benchmark]
    public async Task SqlWildcardAsync()
    {
        using var sqlCommand = new SqlCommand()
        {
            Connection = _sqlConnection,
            CommandText = string.Format(SqlWildcardQuery, _fullTextSearchData.Column, _fullTextSearchData.Table,
                _fullTextSearchData.Column, _searchPhrases[_random.Next(_searchPhrases.Count)],
                _fullTextSearchData.Id, _fullTextSearchData.Offset, _fullTextSearchData.Size),
        };

        using var reader = await sqlCommand.ExecuteReaderAsync();
        await reader.NextResultAsync();
    }

    [GlobalCleanup]
    public void Dispose()
    {
        _container.Dispose();
        _sqlConnection.Dispose();
    }
}
