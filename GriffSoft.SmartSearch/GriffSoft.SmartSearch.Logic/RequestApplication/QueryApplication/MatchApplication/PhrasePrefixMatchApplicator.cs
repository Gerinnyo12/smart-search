using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;
internal class PhrasePrefixMatchApplicator : MatchApplicator
{
    public PhrasePrefixMatchApplicator(string fieldName, string fieldValue) : base(fieldName, fieldValue) { }

    public override void ApplyMatchOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        queryDescriptor
            .MatchPhrasePrefix(m => m
                .Field(_fieldName)
                .Query(_fieldValue));
    }
}
