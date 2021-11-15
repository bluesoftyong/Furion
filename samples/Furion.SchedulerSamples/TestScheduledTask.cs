using Furion.SchedulerJob;

namespace Furion.SchedulerSamples;

[CronTrigger("cron_job", "* * * * *")]
public class TestScheduledTask : IJob
{
    public string Schedule => "* * * * *";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        await Task.CompletedTask;
    }
}