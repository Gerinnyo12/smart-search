using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.DataRead;
internal interface IDataReader<T> where T : class
{
    Task ProcessDataAsync(Func<List<T>, Task> postProcessCallback);
}