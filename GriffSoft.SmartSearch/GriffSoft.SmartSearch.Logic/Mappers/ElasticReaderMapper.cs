using GriffSoft.SmartSearch.Logic.Dtos;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Mappers;
internal class ElasticReaderMapper : IDataReaderMapper<ElasticDocument>
{
    private readonly ElasticMapperProperties _elasticMapperProperites;

    public ElasticReaderMapper(ElasticMapperProperties elasticMapperProperites)
    {
        _elasticMapperProperites = elasticMapperProperites;
    }

    public async Task<List<ElasticDocument>> MapDataSetAsync(DbDataReader dataReader)
    {
        var documents = new List<ElasticDocument>();

        while (await dataReader.ReadAsync())
        {
            var document = MapDataToDocument(dataReader);
            documents.Add(document);
        }

        return documents;
    }

    private ElasticDocument MapDataToDocument(DbDataReader dataReader)
    {
        return new ElasticDocument
        {
            Server = _elasticMapperProperites.Server,
            Database = _elasticMapperProperites.Database,
            Table = _elasticMapperProperites.Table,
            Type = (int)_elasticMapperProperites.Type,
            Column = _elasticMapperProperites.Column,
            Keys = MapIds(dataReader),
            Value = (string)dataReader[_elasticMapperProperites.Column],
        };
    }

    private Dictionary<string, object> MapIds(DbDataReader dataReader)
    {
        var idsDictionary = new Dictionary<string, object>();

        foreach (var key in _elasticMapperProperites.Keys)
        {
            if (!idsDictionary.TryGetValue(key, out _))
            {
                idsDictionary[key] = dataReader[key];
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
