using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.AndApplication;
internal class AndApplicator
{
    private readonly AndMatchTypeConverter _andMatchTypeConverter;

    public AndApplicator(SearchAnd searchAnd)
    {
        _andMatchTypeConverter = new AndMatchTypeConverter(searchAnd);
    }

    public void ApplyAndOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        var andMatchApplicator = _andMatchTypeConverter.ToAndMatchApplicator();
        andMatchApplicator.ApplyMatchOn(queryDescriptor);
    }
}
