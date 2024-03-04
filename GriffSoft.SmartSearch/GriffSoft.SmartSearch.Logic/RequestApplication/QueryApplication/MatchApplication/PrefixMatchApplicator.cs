using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;
internal class PrefixMatchApplicator : MatchApplicator
{
    public PrefixMatchApplicator(string fieldName, string fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatchOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .Prefix(p => p
                .Field(_fieldName)
                .Value(_fieldValue)
                .CaseInsensitive());
    }
}
