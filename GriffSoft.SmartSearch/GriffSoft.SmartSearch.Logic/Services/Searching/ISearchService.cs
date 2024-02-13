using GriffSoft.SmartSearch.Logic.Dtos.Searching;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services.Searching;
public interface ISearchService<T> where T : class
{
    Task<SearchResult<T>> SearchAsync(SearchRequest searchRequest);
}
