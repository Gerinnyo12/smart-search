using GriffSoft.SmartSearch.Logic.Dtos;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services;
public interface ISearchService<T> where T : class
{
    Task<SearchResult<T>> SearchAsync(SearchRequest searchRequest);
}
