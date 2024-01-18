using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.DataProcess;
internal interface IDataReader<T> where T : class
{
    /// <summary>
    /// Queryies data in batches and executes the given <paramref name="endOfBatchAction"/> after every queried batch.
    /// </summary>
    /// <param name="batchSize">The batch size to process the data in.</param>
    /// <param name="endOfBatchAction">The action to execute after every processed batch.</param>
    Task ProcessDataInBatchesAsync(int batchSize, Func<IEnumerable<T>, Task> endOfBatchAction);
}