using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.Appliers.MatchAppliers;
internal class WildcardMatchApplier : MatchApplier
{
    public WildcardMatchApplier(object fieldValue) : base(fieldValue) { }

    public WildcardMatchApplier(string? fieldName, object fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatch(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .MatchPhrasePrefix(c => c
                .Field(_fieldName)
                .Query((string)_fieldValue));
    }
}
