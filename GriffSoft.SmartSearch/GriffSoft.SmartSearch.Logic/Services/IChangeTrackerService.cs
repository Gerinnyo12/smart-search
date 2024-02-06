using System;
using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services;
public interface IChangeTrackerService<T>
{
    DateTime LastSynchonizationDate {  get; }

    Task TrackChangesAsync();
}
