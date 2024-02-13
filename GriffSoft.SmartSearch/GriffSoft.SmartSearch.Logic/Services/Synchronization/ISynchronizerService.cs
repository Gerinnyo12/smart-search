using System;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services.Synchronization;
public interface ISynchronizerService
{
    DateTime LastSynchonizationDate { get; }

    Task SynchronizeAsync();
}
