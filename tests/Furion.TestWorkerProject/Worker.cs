
using Furion.TestWorkerProject.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Furion.TestWorkerProject;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITestService _testService;

    public Worker(ILogger<Worker> logger
        , ITestService testService)
    {
        _logger = logger;
        _testService = testService;
    }

    [AutowiredServices]
    IApp? App { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}
