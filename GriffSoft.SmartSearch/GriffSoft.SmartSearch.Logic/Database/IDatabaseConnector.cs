using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal interface IDatabaseConnector<T> : IDisposable where T : DbConnection
{
    Task<T> Connection { get; }
}
