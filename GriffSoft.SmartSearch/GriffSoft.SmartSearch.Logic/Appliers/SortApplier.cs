using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Enums;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System;
using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Appliers;
internal class SortApplier
{
    private static readonly Dictionary<SortDirection, SortOrder> _sortDirectionMapper = new()
    {
        { SortDirection.Ascending, SortOrder.Asc },
        { SortDirection.Descending, SortOrder.Desc },
    };

    private readonly IEnumerable<SearchSort> _searchSorts;

    public SortApplier(IEnumerable<SearchSort> searchSorts)
    {
        _searchSorts = searchSorts;
    }

    public void ApplySorts(SortOptionsDescriptor<ElasticDocument> searchOptionsDescriptor)
    {
        foreach (var searchSort in _searchSorts)
        {
            searchOptionsDescriptor.Field(searchSort.FieldName, new FieldSort
            {
                Order = TransformSortDirection(searchSort.SortDirection),
            });
        }
    }

    private SortOrder TransformSortDirection(SortDirection sortDirection)
    {
        if (!_sortDirectionMapper.TryGetValue(sortDirection, out var sortOrder))
        {
            throw new Exception($"No query mapping found for {sortDirection}.");
        }

        return sortOrder;
    }
}
