using Furion.SchedulerJob;
using Furion.TimeCrontab;

namespace Furion.SchedulerSamples;

[CronJob("cron_seconds_job", "* * * * * *", Format = CronStringFormat.WithSeconds)]
public class TestCronWithSecondsJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"<{context.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        await Task.CompletedTask;
    }
}