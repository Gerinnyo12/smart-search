using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.SortApplication;
internal class SortApplicator
{
    private readonly SearchSort _searchSort;
    private readonly SortDirectionConverter _sortDirectionConverter;

    public SortApplicator(SearchSort searchSort)
    {
        _searchSort = searchSort;
        _sortDirectionConverter = new SortDirectionConverter(_searchSort.SortDirection);
    }

    public void ApplyOn(SortOptionsDescriptor<ElasticDocument> sortOptionsDescriptor)
    {
        sortOptionsDescriptor.Field(_searchSort.FieldName, new FieldSort
        {
            Order = _sortDirectionConverter.ConvertSortDirection(),
        });
    }
}
