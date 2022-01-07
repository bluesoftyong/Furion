using Furion.Schedule;

namespace Furion.SchedulerSamples;

public class TestPeriodJob : IJob
{
    private readonly ILogger<TestPeriodJob> _logger;

    public TestPeriodJob(ILogger<TestPeriodJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"<{context.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {context.JobTrigger!.NumberOfRuns} times");

        await Task.CompletedTask;
    }
}