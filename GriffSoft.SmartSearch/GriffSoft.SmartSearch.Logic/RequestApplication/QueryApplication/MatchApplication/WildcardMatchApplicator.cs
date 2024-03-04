using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Extensions;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;
internal class WildcardMatchApplicator : MatchApplicator
{
    private const string Wildcard = ".*";

    public WildcardMatchApplicator(string fieldName, string fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatchOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        string regex = (_fieldValue).SurroundWith("\"").SurroundWith(Wildcard);
        queryDescriptor
            .Regexp(c => c
                .Field(_fieldName)
                .Value(regex)
                .CaseInsensitive()
                .Flags("NONE"));
    }
}
