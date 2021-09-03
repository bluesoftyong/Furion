using Furion.TestWorkerProject;

IHost host = Host.CreateDefaultBuilder(args)
    .UseFurion()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();