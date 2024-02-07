using GriffSoft.SmartSearch.Logic.Dtos;

using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GriffSoft.SmartSearch.Logic.Factories;
internal class IdHashFactory : IFactory<string>
{
    private readonly ElasticDocumentIdHashDto _elasticDocumentIdHashDto;

    public IdHashFactory(ElasticDocumentIdHashDto elasticDocumentIdHashDto)
    {
        _elasticDocumentIdHashDto = elasticDocumentIdHashDto;
    }

    public string Create()
    {
        string json = JsonSerializer.Serialize(_elasticDocumentIdHashDto);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using var hashAlgorithm = MD5.Create();
        var hashedBytes = hashAlgorithm.ComputeHash(jsonBytes);
        string hash = string.Concat(hashedBytes.Select(b => b.ToString("X2")));

        return hash;
    }
}
