using Furion.Schedule;

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
        _logger.LogInformation($"<{context.JobDetail.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {context.JobTrigger.NumberOfRuns} times");

        await Task.CompletedTask;
    }
}