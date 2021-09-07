using Furion.DependencyInjection;
using Furion.TestProject.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Furion.TestProject;

public class TestHostBuilderServices : IHostBuilderService
{
    void IHostBuilderService.Configure(IServiceCollection services, HostBuilderContext context)
    {
        services.AsServiceBuilder(context).TryAddAssemblies(typeof(FakeStartup).Assembly);
        services.AsServiceBuilder(context)
            .AddNamedService<Test1NamedService>("test1", ServiceLifetime.Transient)
            .AddNamedService<Test2NamedService>("test2", ServiceLifetime.Transient);

        services.AsServiceBuilder(context)
            .AddNamedService<Test1NamedService>("test3", ServiceLifetime.Transient)
            .AddNamedService<Test2NamedService>("test4", ServiceLifetime.Transient);
    }
}