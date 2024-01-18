using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.Factories;
internal interface IQueryFactory<T> where T : DbCommand
{
    Task<T> CreatePaginatedQueryAsync(int size, int page);
}
