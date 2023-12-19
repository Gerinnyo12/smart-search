using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal interface IDataReader<T> where T : class
{
    Task ProcessDataInBatchesAsync(Func<IEnumerable<T>, Task> endOfBatchAction);
}