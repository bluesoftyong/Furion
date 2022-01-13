using Furion.Schedule;
using System.Diagnostics;
using System.Text.Json;

namespace Furion.SchedulerSamples;

public class TestCronJob : IJob
{
    private readonly ILogger<TestCronJob> _logger;

    public TestCronJob(ILogger<TestCronJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        Trace.WriteLine(JsonSerializer.Serialize(context.JobDetail));
        Trace.WriteLine(JsonSerializer.Serialize(context.JobTrigger));
        _logger.LogInformation($"<{context.JobDetail.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {context.JobTrigger.TriggerId} {context.JobTrigger.NumberOfRuns} times");

        await Task.CompletedTask;
    }
}