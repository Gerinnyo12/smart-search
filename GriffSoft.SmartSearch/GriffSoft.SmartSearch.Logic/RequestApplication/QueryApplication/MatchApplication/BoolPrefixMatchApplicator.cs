using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;
internal class BoolPrefixMatchApplicator : MatchApplicator
{
    public BoolPrefixMatchApplicator(string fieldName, string fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatchOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .MatchBoolPrefix(p => p
                .Field(_fieldName)
                .Query(_fieldValue));
    }
}
