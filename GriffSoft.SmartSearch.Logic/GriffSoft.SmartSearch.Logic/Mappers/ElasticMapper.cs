using GriffSoft.SmartSearch.Logic.Dtos;

using Microsoft.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Mappers;
internal class ElasticMapper : IDataReaderMapper<ElasticDocument>
{
    private readonly ElasticMapperParameters _elasticMapperParameters;

    public ElasticMapper(ElasticMapperParameters elasticMapperParameters)
    {
        _elasticMapperParameters = elasticMapperParameters;
    }

    public async Task<List<ElasticDocument>> MapDataSetAsync(DbDataReader dataReader)
    {
        if (dataReader is not SqlDataReader sqlDataReader)
        {
            throw new Exception($"The provided data reader is not an instance of {nameof(SqlDataReader)}");
        }

        var documents = new List<ElasticDocument>();

        while (await dataReader.ReadAsync())
        {
            var document = MapDataToDocument(sqlDataReader);
            documents.Add(document);
        }

        return documents;
    }

    private ElasticDocument MapDataToDocument(SqlDataReader sqlDataReader)
    {
        return new ElasticDocument
        {
            Server = _elasticMapperParameters.Server,
            Database = _elasticMapperParameters.Database,
            Table = _elasticMapperParameters.Table,
            Column = _elasticMapperParameters.Column,
            Keys = MapIds(sqlDataReader),
            Value = sqlDataReader[_elasticMapperParameters.Column],
        };
    }

    private Dictionary<string, object> MapIds(SqlDataReader sqlDataReader)
    {
        var idsDictionary = new Dictionary<string, object>();

        foreach (var key in _elasticMapperParameters.Keys)
        {
            if (!idsDictionary.TryGetValue(key, out _))
            {
                idsDictionary[key] = sqlDataReader[key];
            }
            else
            {
                // TODO
                throw new Exception($"{key} is already in the dictionary.");
            }
        }

        return idsDictionary;
    }
}
