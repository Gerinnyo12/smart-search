using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Database.Connection;
internal interface IDatabaseConnector<T> : IDisposable where T : DbConnection
{
    Task<T> Connection { get; }
}
