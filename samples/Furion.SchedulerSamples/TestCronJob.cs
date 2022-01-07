using Furion.Schedule;

namespace Furion.SchedulerSamples;

public class TestCronJob : IJob
{
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"<{context.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {context.JobTrigger!.NumberOfRuns} times");

        await Task.CompletedTask;
    }
}