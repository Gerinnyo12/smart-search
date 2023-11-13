using GriffSoft.SmartSearch.Logic.Mappers;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal class DataReader<T>
{
    private readonly IQueryBuilder _queryBuilder;
    private readonly IDataReaderMapper<T> _dataReaderMapper;

    public DataReader(IQueryBuilder queryBuilder, IDataReaderMapper<T> dataReaderMapper)
    {
        _queryBuilder = queryBuilder;
        _dataReaderMapper = dataReaderMapper;
    }

    public async Task ProcessDataInBatchesAsync(Func<IEnumerable<T>, Task> endOfBatchAction)
    {
        int page = 0;
        bool hasUnprocessedData = true;

        while (hasUnprocessedData)
        {
            using var query = await _queryBuilder.BuildQueryForPage(page);
            var documents = await QueryDocumentsAsync(query);
            await endOfBatchAction(documents);

            page++;
            hasUnprocessedData = documents.Any();
        }
    }

    private async Task<List<T>> QueryDocumentsAsync(DbCommand query)
    {
        using var dataReader = await query.ExecuteReaderAsync();
        var documents = await _dataReaderMapper.MapDataSetAsync(dataReader);
        return documents;
    }
}
