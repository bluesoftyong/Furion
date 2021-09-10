using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Furion.UnitTests;

/// <summary>
/// IApp 多注册实例
/// </summary>
public class AppRepeat : IApp
{
    public IServiceProvider ServiceProvider => throw new NotImplementedException();

    public IConfiguration Configuration => throw new NotImplementedException();

    public IHostEnvironment Environment => throw new NotImplementedException();

    public IHost Host => throw new NotImplementedException();

    public object GetRequiredService(Type serviceType)
    {
        throw new NotImplementedException();
    }

    public TService GetRequiredService<TService>() where TService : class
    {
        throw new NotImplementedException();
    }

    public object? GetService(Type serviceType)
    {
        throw new NotImplementedException();
    }

    public TService? GetService<TService>() where TService : class
    {
        throw new NotImplementedException();
    }
}