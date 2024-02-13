using GriffSoft.SmartSearch.Database.Mapping;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Synchronization;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Mapping;
internal class ElasticDocumentMapper : IDataMapper<ElasticDocument>
{
    private readonly ElasticDocumentMapperDto _elasticDocumentMapperDto;

    public ElasticDocumentMapper(ElasticDocumentMapperDto elasticDocumentMapperDto)
    {
        _elasticDocumentMapperDto = elasticDocumentMapperDto;
    }

    public async Task<List<ElasticDocument>> MapAsync(DbDataReader dataReader)
    {
        var documentCollection = new List<ElasticDocument>();

        while (await dataReader.ReadAsync())
        {
            var elasticDocuments = MapToDocument(dataReader);
            documentCollection.AddRange(elasticDocuments);
        }

        return documentCollection;
    }

    private List<ElasticDocument> MapToDocument(DbDataReader dataReader)
    {
        var elasticDocuments = new List<ElasticDocument>();

        foreach (var column in _elasticDocumentMapperDto.Columns)
        {
            var elasticDocument = new ElasticDocument
            {
                Server = _elasticDocumentMapperDto.Server,
                Database = _elasticDocumentMapperDto.Database,
                Table = _elasticDocumentMapperDto.Table,
                Type = _elasticDocumentMapperDto.Type,
                Keys = MapIds(dataReader),
                Column = column,
                Value = dataReader[column].ToString(),
            };
            elasticDocuments.Add(elasticDocument);
        }

        return elasticDocuments;
    }

    private Dictionary<string, object> MapIds(DbDataReader dataReader)
    {
        var idsDictionary = new Dictionary<string, object>();

        foreach (var key in _elasticDocumentMapperDto.Keys)
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
