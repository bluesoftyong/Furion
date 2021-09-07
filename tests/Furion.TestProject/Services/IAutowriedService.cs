using Microsoft.Extensions.DependencyInjection;

namespace Furion.TestProject.Services;

public interface IAutowriedService
{
    IApp? App { get; set; }
}

public class AutowriedService : IAutowriedService
{
    [AutowiredServices]
    public IApp? App { get; set; }
}