using Furion.TestWorkerProject;
using Furion.TestWorkerProject.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .UseFurion()
    .ConfigureServices((hostContext, services) =>
    {
        // Add services to the container.
        services.AddSingleton<ITestService, TestService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();