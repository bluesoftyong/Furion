using Furion.SchedulerJob;

namespace Furion.SchedulerSamples;

[SimpleJob("simple_job", 1000)]
public class TestSimpleJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"<{context.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        await Task.CompletedTask;
    }
}