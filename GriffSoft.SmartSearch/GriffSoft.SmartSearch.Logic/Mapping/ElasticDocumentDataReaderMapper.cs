using GriffSoft.SmartSearch.Database.Mapping;
using GriffSoft.SmartSearch.Logic.Dtos;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Mapping;
internal class ElasticDocumentDataReaderMapper : IDataReaderMapper<ElasticDocument>
{
    private readonly DataReaderDto _dataReaderDto;

    public ElasticDocumentDataReaderMapper(DataReaderDto dataReaderDto)
    {
        _dataReaderDto = dataReaderDto;
    }

    public async Task<List<ElasticDocument>> MapAsync(DbDataReader dataReader)
    {
        var documents = new List<ElasticDocument>();

        while (await dataReader.ReadAsync())
        {
            var document = MapToDocument(dataReader);
            documents.Add(document);
        }

        return documents;
    }

    private ElasticDocument MapToDocument(DbDataReader dataReader)
    {
        return new ElasticDocument
        {
            Server = _dataReaderDto.Server,
            Database = _dataReaderDto.Database,
            Table = _dataReaderDto.Table,
            Type = _dataReaderDto.Type,
            Column = _dataReaderDto.Column,
            Keys = MapIds(dataReader),
            Value = dataReader[_dataReaderDto.Column].ToString(),
        };
    }

    private Dictionary<string, object> MapIds(DbDataReader dataReader)
    {
        var idsDictionary = new Dictionary<string, object>();

        foreach (var key in _dataReaderDto.Keys)
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
