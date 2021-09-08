
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Furion.TestWorkerProject;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IAppServiceProvider _appServiceProvider;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger
        , IAppServiceProvider appServiceProvider
        , IServiceProvider serviceProvider)
    {
        _logger = logger;
        _appServiceProvider = appServiceProvider;
        _serviceProvider = serviceProvider;
    }

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
