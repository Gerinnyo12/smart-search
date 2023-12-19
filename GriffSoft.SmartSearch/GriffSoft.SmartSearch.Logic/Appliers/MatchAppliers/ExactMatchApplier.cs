using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.Appliers.MatchAppliers;
internal class ExactMatchApplier : MatchApplier
{
    public ExactMatchApplier(object fieldValue) : base(fieldValue) { }

    public ExactMatchApplier(string? fieldName, object fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatch(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .MatchPhrase(m => m
                .Field(_fieldName)
                .Query((string)_fieldValue));
    }
}
