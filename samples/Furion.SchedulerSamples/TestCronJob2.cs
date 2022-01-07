using Furion.Schedule;

namespace Furion.SchedulerSamples;

public class TestCronJob2 : IJob
{
    private readonly ILogger<TestCronJob2> _logger;

    public TestCronJob2(ILogger<TestCronJob2> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"<{context.JobDetail.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {context.JobTrigger.NumberOfRuns} times");

        await Task.CompletedTask;
    }
}