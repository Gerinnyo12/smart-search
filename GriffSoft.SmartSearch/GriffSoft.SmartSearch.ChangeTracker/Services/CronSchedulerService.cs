using GriffSoft.SmartSearch.ChangeTracker.Options;
using GriffSoft.SmartSearch.Logic.Dtos;
using GriffSoft.SmartSearch.Logic.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GriffSoft.SmartSearch.ChangeTracker.Services;

public class CronSchedulerService(IChangeTrackerService<ElasticDocument> changeTrackerService,
    IOptions<CronSchedulerOptions> cronSchedulerOptions, ILogger<CronSchedulerService> logger) : BackgroundService, ICronSchedulerService
{
    private readonly IChangeTrackerService<ElasticDocument> _changeTrackerService = changeTrackerService;
    private readonly CronSchedulerOptions _cronSchedulerOptions = cronSchedulerOptions.Value;
    private readonly ILogger<CronSchedulerService> _logger = logger;

    public TimeSpan NextOccurance => GetNextOccurance();

    private TimeSpan GetNextOccurance()
    {
        var nextOccurance = _cronSchedulerOptions.CronExpression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
        var differenceTimeSpan = nextOccurance?.Subtract(DateTimeOffset.Now) ?? TimeSpan.FromDays(1);
        return differenceTimeSpan;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunJobsAsync();
            await Task.Delay(NextOccurance, stoppingToken);
        }
    }

    public async Task RunJobsAsync()
    {
        _logger.LogInformation("Start running sheduled jobs.");

        try
        {
            await _changeTrackerService.TrackChangesAsync();
            _logger.LogInformation("Every scheduled job finished running.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occured running the scheduled jobs.");
        }
    }
}
