using GriffSoft.SmartSearch.Database.Dtos;

using Microsoft.Data.SqlClient;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.Factories;
public abstract class SqlBatchQueryFactory : IBatchQueryFactory<SqlCommand>
{
    protected readonly SqlQueryDto _sqlQueryDto;
    protected int _batchCount = 0;

    public SqlBatchQueryFactory(SqlQueryDto sqlQueryDto)
    {
        _sqlQueryDto = sqlQueryDto;
    }

    public virtual async Task<SqlCommand> CreateNextAsync()
    {
        string paramaterisedSqlQuery = GetParamaterisedSqlQuery();
        var sqlConnection = await _sqlQueryDto.SqlConnector.Connection;
        var sqlCommand = new SqlCommand
        {
            Connection = sqlConnection,
            CommandText = paramaterisedSqlQuery,
        };

        _batchCount++;
        return sqlCommand;
    }

    protected abstract string GetParamaterisedSqlQuery();
}
