using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal interface IDatabaseQueryBuilder<T> where T : DbCommand
{
    Task<T> BuildQueryForPageAsync(int page);
}
