using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Mappers;
internal interface IDataReaderMapper<T>
{
    Task<List<T>> MapDataSetAsync(DbDataReader dataReader);
}
