using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.Mapping;
public interface IDataReaderMapper<T>
{
    Task<List<T>> MapAsync(DbDataReader dataReader);
}
