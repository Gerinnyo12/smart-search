using GriffSoft.SmartSearch.Database.Factories.Sql;
using GriffSoft.SmartSearch.Database.Mapping;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.DataProcess.Sql;
public class SqlDataReader<T> : IDataReader<T> where T : class
{
    private readonly SqlQueryFactory _sqlQueryFactory;
    private readonly IDataReaderMapper<T> _elasticReaderMapper;

    // TODO Mapper intefacet kapjon konkret implementacio helyett
    public SqlDataReader(SqlQueryFactory sqlQueryFactory, IDataReaderMapper<T> elasticReaderMapper)
    {
        _sqlQueryFactory = sqlQueryFactory;
        _elasticReaderMapper = elasticReaderMapper;
    }

    public async Task ProcessDataInBatchesAsync(int batchSize, Func<IEnumerable<T>, Task> endOfBatchAction)
    {
        int page = 0;
        bool hasUnprocessedData = true;

        while (hasUnprocessedData)
        {
            using var query = await _sqlQueryFactory.CreatePaginatedQueryAsync(batchSize, page);
            var documents = await QueryDocumentsAsync(query);
            await endOfBatchAction(documents);

            page++;
            hasUnprocessedData = documents.Any();
        }
    }

    private async Task<List<T>> QueryDocumentsAsync(DbCommand query)
    {
        using var dataReader = await query.ExecuteReaderAsync();
        var documents = await _elasticReaderMapper.MapAsync(dataReader);
        return documents;
    }
}
