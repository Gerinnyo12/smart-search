using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;
internal class TermMatchApplicator : MatchApplicator
{
    public TermMatchApplicator(string fieldName, string fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatchOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .Term(t => t
                .Field(_fieldName)
                .Value(_fieldValue)
                .CaseInsensitive());
    }
}
