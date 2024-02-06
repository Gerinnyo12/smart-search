using GriffSoft.SmartSearch.Database.Factories;
using GriffSoft.SmartSearch.Database.Mapping;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.DataRead;
public class SqlBatchDataReader<T> : IDataReader<T> where T : class
{
    private readonly SqlBatchQueryFactory _sqlBatchQueryFactory;
    private readonly IDataMapper<T> _dataMapper;

    public SqlBatchDataReader(SqlBatchQueryFactory sqlBatchQueryFactory, IDataMapper<T> dataMapper)
    {
        _sqlBatchQueryFactory = sqlBatchQueryFactory;
        _dataMapper = dataMapper;
    }

    public async Task ProcessDataAsync(Func<List<T>, Task> postProcessCallback)
    {
        bool hasUnprocessedData = true;

        while (hasUnprocessedData)
        {
            using var query = await _sqlBatchQueryFactory.CreateNextAsync();
            var data = await QueryDataAsync(query);
            await postProcessCallback(data);

            hasUnprocessedData = data.Any();
        }
    }

    private async Task<List<T>> QueryDataAsync(DbCommand query)
    {
        using var dataReader = await query.ExecuteReaderAsync();
        var documents = await _dataMapper.MapAsync(dataReader);
        return documents;
    }
}
