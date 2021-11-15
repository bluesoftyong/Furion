using Furion.SchedulerJob;
using System.Text.Json;

namespace Furion.SchedulerSamples;

[SimpleJob("simple_job", 1000)]
public class TestSimpleJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"<{context.JobDetail.Identity}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {JsonSerializer.Serialize(context.JobDetail)}");

        await Task.CompletedTask;
    }
}