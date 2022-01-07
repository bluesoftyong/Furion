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
        if (context.JobTrigger.NumberOfRuns == 5)
        {
            throw new Exception("出错啦");
        }

        _logger.LogInformation($"<{context.JobDetail.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {context.JobTrigger.NumberOfRuns} times");

        await Task.CompletedTask;
    }
}