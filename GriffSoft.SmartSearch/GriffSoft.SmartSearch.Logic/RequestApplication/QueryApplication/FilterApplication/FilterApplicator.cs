using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.FilterApplication;
internal class FilterApplicator
{
    private readonly FilterMatchTypeConverter _filterMatchTypeConverter;

    public FilterApplicator(SearchFilter searchFilter)
    {
        _filterMatchTypeConverter = new FilterMatchTypeConverter(searchFilter);
    }

    public void ApplyFilterOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        var filterMatchApplicator = _filterMatchTypeConverter.ToFilterMatchApplicator();
        filterMatchApplicator.ApplyMatchOn(queryDescriptor);
    }
}
