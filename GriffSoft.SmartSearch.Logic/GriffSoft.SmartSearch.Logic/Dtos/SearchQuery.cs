using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public record SearchQuery()
{
    private const string ValueFieldName = "value";

    public required string Query { get; set; }

    public Field[] Fields { get; set; } = new Field[] {
        ValueFieldName,
        $"{ValueFieldName}._2gram",
        $"{ValueFieldName}._3gram"
    };

    public TextQueryType Type { get; set; } = TextQueryType.BoolPrefix;

    public static implicit operator SearchQuery(string query) => new() { Query = query};
}
