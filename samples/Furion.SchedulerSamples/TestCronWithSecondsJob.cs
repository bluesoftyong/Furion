using Furion.SchedulerJob;
using Furion.TimeCrontab;
using System.Text.Json;

namespace Furion.SchedulerSamples;

[CronJob("cron_seconds_job", "* * * * * *", Format = CronStringFormat.WithSeconds)]
public class TestCronWithSecondsJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"<{context.JobDetail.Identity}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {JsonSerializer.Serialize(context.JobDetail)}");

        await Task.CompletedTask;
    }
}