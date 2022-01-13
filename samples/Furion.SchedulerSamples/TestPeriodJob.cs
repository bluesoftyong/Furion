using Furion.Schedule;

namespace Furion.SchedulerSamples;

[SandboxScope]
public class TestPeriodJob : IJob
{
    private readonly ILogger<TestPeriodJob> _logger;
    private readonly ITestScopedService _service;
    private readonly ITestTransientService _service2;

    public TestPeriodJob(ILogger<TestPeriodJob> logger
        , ITestScopedService service
        , ITestTransientService service2)
    {
        _logger = logger;
        _service = service;
        _service2 = service2;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        if (context.JobTrigger.NumberOfRuns == 5)
        {
            throw new Exception("出错啦");
        }

        // 测试服务释放
        _service.Test();
        _service2.Test();

        _logger.LogInformation($"<{context.JobDetail.JobId}> {DateTime.Now:yyyy-MM-dd HH:mm:ss} {context.JobTrigger.TriggerId} {context.JobTrigger.NumberOfRuns} times");

        await Task.CompletedTask;
    }
}