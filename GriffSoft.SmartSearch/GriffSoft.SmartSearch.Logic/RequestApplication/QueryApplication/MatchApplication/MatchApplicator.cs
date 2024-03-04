using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.MatchApplication;
internal abstract class MatchApplicator : IMatchApplicator
{
    protected readonly string _fieldName;
    protected readonly string _fieldValue;

    public MatchApplicator(string fieldName, string fieldValue)
    {
        _fieldName = fieldName;
        _fieldValue = fieldValue;
    }

    public abstract void ApplyMatchOn(QueryDescriptor<ElasticDocument> queryDescriptor);
}
