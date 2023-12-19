using Elastic.Clients.Elasticsearch.QueryDsl;

using GriffSoft.SmartSearch.Logic.Dtos;

namespace GriffSoft.SmartSearch.Logic.Appliers.MatchAppliers;
internal abstract class MatchApplier
{
    protected const string SEARCH_FIELD_NAME = "Value";
    protected readonly string _fieldName;
    protected readonly object _fieldValue;

    public MatchApplier(string? fieldName, object fieldValue)
    {
        _fieldName = fieldName ?? SEARCH_FIELD_NAME;
        _fieldValue = fieldValue;
    }

    public MatchApplier(object fieldValue) : this(SEARCH_FIELD_NAME, fieldValue) { }

    public abstract void ApplyMatch(QueryDescriptor<ElasticDocument> queryDescriptor);
}
