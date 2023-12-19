using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.Appliers.MatchAppliers;
internal class PrefixMatchApplier : MatchApplier
{
    public PrefixMatchApplier(object fieldValue) : base(fieldValue) { }

    public PrefixMatchApplier(string? fieldName, object fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatch(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .Prefix(p => p
                .Field(_fieldName)
                .Value((string)_fieldValue));
    }
}
