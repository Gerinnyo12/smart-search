using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.Appliers.MatchAppliers;
internal class SearchAsYouTypeMatchApplier : MatchApplier
{
    public SearchAsYouTypeMatchApplier(object fieldValue) : base(fieldValue) { }

    public SearchAsYouTypeMatchApplier(string? fieldName, object fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatch(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .MultiMatch(mqd => mqd
                .Type(TextQueryType.BoolPrefix)
                .Fields(new[] { _fieldName, $"{_fieldName}._2gram", $"{_fieldName}._3gram" })
                .Query((string)_fieldValue));
    }
}
