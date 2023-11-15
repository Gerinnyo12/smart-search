using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal interface IQueryBuilder
{
    Task<DbCommand> BuildQueryForPageAsync(int page);
}
