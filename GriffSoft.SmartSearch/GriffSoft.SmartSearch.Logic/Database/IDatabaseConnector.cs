using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Database;
internal interface IDatabaseConnector : IDisposable
{
    Task<DbConnection> Connection { get; }
}
