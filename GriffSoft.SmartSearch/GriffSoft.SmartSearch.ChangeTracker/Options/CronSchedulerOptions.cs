using Cronos;

using GriffSoft.SmartSearch.ChangeTracker.Exceptions;
using GriffSoft.SmartSearch.Logic.Options;

namespace GriffSoft.SmartSearch.ChangeTracker.Options;
public class CronSchedulerOptions : IValidatable
{
    private CronExpression? _cronExpression;

    public required string Cron { get; init; }

    public CronExpression CronExpression => ParseCronExpression();

    public void InvalidateIfIncorrect()
    {
        ParseCronExpression();
    }

    public CronExpression ParseCronExpression()
    {
        if (_cronExpression is not null)
        {
            return _cronExpression;
        }

        if (!CronExpression.TryParse(Cron, out CronExpression cronExpression))
        {
            throw new CronParseException($"'{Cron}' is not a valid cron.");
        }

        _cronExpression = cronExpression;
        return cronExpression;
    }
}
