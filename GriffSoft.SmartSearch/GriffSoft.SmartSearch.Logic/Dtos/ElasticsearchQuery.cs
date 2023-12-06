using Elastic.Clients.Elasticsearch;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class ElasticsearchQuery<T> : PaginatedSearchQuery
{
    public required SortOptionsDescriptor<T> SortDescriptor { get; init; }
}
