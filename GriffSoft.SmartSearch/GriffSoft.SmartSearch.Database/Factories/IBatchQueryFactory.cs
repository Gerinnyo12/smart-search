using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.Factories;
public interface IBatchQueryFactory<T> where T : DbCommand
{
    Task<T> CreateNextAsync();
}
