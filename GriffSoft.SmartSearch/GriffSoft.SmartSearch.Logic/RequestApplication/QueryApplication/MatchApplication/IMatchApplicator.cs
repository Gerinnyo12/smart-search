using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;
internal interface IMatchApplicator
{
    void ApplyMatchOn(QueryDescriptor<ElasticDocument> queryDescriptor);
}
