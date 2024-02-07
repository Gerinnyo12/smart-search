namespace GriffSoft.SmartSearch.ChangeTracker.Services;
public interface ICronSchedulerService
{
    TimeSpan NextOccurance { get; }

    Task RunJobsAsync();
}
