using GriffSoft.SmartSearch.Logic.Dtos.Searching;
using GriffSoft.SmartSearch.Logic.Exceptions;

using SearchRequest = GriffSoft.SmartSearch.Logic.Dtos.Searching.SearchRequest;

namespace GriffSoft.SmartSearch.Logic.Builders;
public class SearchRequestBuilder : IBuilder<SearchRequest>
{
    private readonly SearchRequest _searchRequest = new();

    public SearchRequest Build()
    {
        if (_searchRequest.Size == 0)
        {
            throw new MissingRequestPropertyException($"{nameof(_searchRequest.Size)} must be set.");
        }

        return _searchRequest;
    }

    public SearchRequestBuilder Filters(params SearchFilter[] searchFilters)
    {
        _searchRequest.Filters.AddRange(searchFilters);
        return this;
    }

    public SearchRequestBuilder Ands(params SearchAnd[] searchAnds)
    {
        _searchRequest.Ands.AddRange(searchAnds);
        return this;
    }

    public SearchRequestBuilder Ors(params SearchOr[] searchEnds)
    {
        _searchRequest.Ors.AddRange(searchEnds);
        return this;
    }

    public SearchRequestBuilder Sorts(params SearchSort[] searchSorts)
    {
        _searchRequest.Sorts.AddRange(searchSorts);
        return this;
    }

    public SearchRequestBuilder Take(int size)
    {
        _searchRequest.Size = size;
        return this;
    }

    public SearchRequestBuilder Skip(int offset)
    {
        _searchRequest.Offset = offset;
        return this;
    }
}
