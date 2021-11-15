using Furion.SchedulerJob;

namespace Furion.SchedulerSamples;

[CronJob("cron_job", "* * * * *")]
public class TestCronJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"<{context.JobDetail.Identity}> {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        await Task.CompletedTask;
    }
}