using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.Appliers.MatchAppliers;
internal class NumericOrMatchApplier : MatchApplier
{
    public NumericOrMatchApplier(object fieldValue) : base(fieldValue) { }

    public NumericOrMatchApplier(string? fieldName, object fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatch(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        double numericFieldValue = (double)_fieldValue;
        queryDescriptor.Range(r => r
            .NumberRange(d => d
                .Field(_fieldName)
                .Gte(numericFieldValue)
                .Lte(numericFieldValue)));
    }
}
