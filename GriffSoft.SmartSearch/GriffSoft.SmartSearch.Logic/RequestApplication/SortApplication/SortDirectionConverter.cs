using Elastic.Clients.Elasticsearch;

using GriffSoft.SmartSearch.Logic.Dtos.Enums;

using System;
using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.SortApplication;
internal class SortDirectionConverter
{
    private static readonly Dictionary<SortDirection, SortOrder> _sortDirectionMapper = new()
    {
        { SortDirection.Ascending, SortOrder.Asc },
        { SortDirection.Descending, SortOrder.Desc },
    };

    private readonly SortDirection _sortDirection;

    public SortDirectionConverter(SortDirection sortDirection)
    {
        _sortDirection = sortDirection;
    }

    public SortOrder ConvertSortDirection()
    {
        if (!_sortDirectionMapper.TryGetValue(_sortDirection, out var sortOrder))
        {
            throw new Exception($"No query mapping found for {_sortDirection}.");
        }

        return sortOrder;
    }
}
