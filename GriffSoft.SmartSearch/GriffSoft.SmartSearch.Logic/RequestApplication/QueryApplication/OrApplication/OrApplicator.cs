using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Dtos.Searching;

namespace GriffSoft.SmartSearch.Logic.RequestApplication.QueryApplication.OrApplication;
internal class OrApplicator
{
    private readonly OrMatchTypeConverter _orMatchTypeConverter;

    public OrApplicator(SearchOr searchOr)
    {
        _orMatchTypeConverter = new OrMatchTypeConverter(searchOr);
    }

    public void ApplyOrOn(QueryDescriptor<ElasticDocument> queryDescriptor)
    {
        var orMatchApplicator = _orMatchTypeConverter.ToOrMatchApplicator();
        orMatchApplicator.ApplyMatchOn(queryDescriptor);
    }
}
