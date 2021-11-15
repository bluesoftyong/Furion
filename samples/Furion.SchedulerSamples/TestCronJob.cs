using Furion.SchedulerJob;
using System.Text.Json;

namespace Furion.SchedulerSamples;

[CronJob("cron_job", "* * * * *")]
public class TestCronJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"<{context.JobDetail.Identity}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {JsonSerializer.Serialize(context.JobDetail)}");

        await Task.CompletedTask;
    }
}