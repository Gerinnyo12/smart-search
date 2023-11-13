using GriffSoft.SmartSearch.Logic.Dtos;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services;
public interface ISearchService
{
    // TODO DOCCOMMENT ABOUT EXCEPTION THROWING
    Task InitializeDataAsync();

    Task SearchAsync(SearchQuery searchQuery);
}
