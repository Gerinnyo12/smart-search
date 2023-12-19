using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Mappers;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal class SqlDataReader : IDataReader<ElasticDocument>
{
    private readonly ElasticQueryBuilder _elasticQueryBuilder;
    private readonly ElasticReaderMapper _elasticReaderMapper;

    public SqlDataReader(ElasticQueryBuilder databaseQueryBuilder, ElasticReaderMapper elasticReaderMapper)
    {
        _elasticQueryBuilder = databaseQueryBuilder;
        _elasticReaderMapper = elasticReaderMapper;
    }

    public async Task ProcessDataInBatchesAsync(Func<IEnumerable<ElasticDocument>, Task> endOfBatchAction)
    {
        int page = 0;
        bool hasUnprocessedData = true;

        while (hasUnprocessedData)
        {
            using var query = await _elasticQueryBuilder.BuildQueryForPageAsync(page);
            var documents = await QueryDocumentsAsync(query);
            await endOfBatchAction(documents);

            page++;
            hasUnprocessedData = documents.Any();
        }
    }

    private async Task<List<ElasticDocument>> QueryDocumentsAsync(DbCommand query)
    {
        using var dataReader = await query.ExecuteReaderAsync();
        var documents = await _elasticReaderMapper.MapDataSetAsync(dataReader);
        return documents;
    }
}
