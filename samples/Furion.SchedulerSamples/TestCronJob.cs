using Furion.SchedulerJob;

namespace Furion.SchedulerSamples;

[CronJob("cron_job", "* * * * *")]
public class TestCronJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        await Task.CompletedTask;
    }
}