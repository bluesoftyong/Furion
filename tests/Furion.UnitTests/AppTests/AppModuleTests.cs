using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Furion.UnitTests;

/// <summary>
/// App 模块测试
/// </summary>
public class AppModuleTests
{
    /// <summary>
    /// 测试默认注册
    /// </summary>
    [Fact]
    public void TestDefaultRegister()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        using var app = builder.Build();
        var services = app.Services;

        // 测试默认注册
        services.Invoking(s => s.GetRequiredService<IApp>()).Should().NotThrow();
    }

    /// <summary>
    /// 测试手动注册
    /// </summary>
    [Fact]
    public void TestManualRegister()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddApp();
        using var app = builder.Build();
        var services = app.Services;

        // 测试手动注册
        services.Invoking(s => s.GetRequiredService<IApp>()).Should().NotThrow();
        //services.Invoking(s => s.GetRequiredService<IOptions<AppSettingsOptions>>().Value).Should().NotThrow();
    }

    /// <summary>
    /// 测试无注册
    /// </summary>
    [Fact]
    public void TestNotRegister()
    {
        var builder = WebApplication.CreateBuilder();
        using var app = builder.Build();
        var services = app.Services;

        // 测试无注册
        services.Invoking(s => s.GetRequiredService<IApp>()).Should().Throw<InvalidOperationException>();
        //services.Invoking(s => s.GetRequiredService<IOptions<AppSettingsOptions>>().Value).Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// 测试 App 模块环境配置
    /// </summary>
    [Fact]
    public void TestEnvironmentConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        var host = builder.Host;
        host.ConfigureAppConfiguration(builder =>
        {
            builder.Sources.Clear();
        });

        // 添加 App 模块配置
        host.ConfigureAppConfiguration();

        var configuration = builder.Configuration;
        var configurationSources = typeof(IConfigurationBuilder).GetProperty("Sources")!.GetValue(configuration) as IList<IConfigurationSource>;

        // 测试默认环境配置
        configurationSources.Should().Contain(u => u is EnvironmentVariablesConfigurationSource);
        var environmentVariablesConfigurationSource = configurationSources!.First(u => u is EnvironmentVariablesConfigurationSource) as EnvironmentVariablesConfigurationSource;
        environmentVariablesConfigurationSource!.Prefix.Should().BeEquivalentTo("FURION_");
    }

    /// <summary>
    /// 测试 App 模块自定义配置文件
    /// </summary>
    [Fact]
    public void TestCustomizeConfiguration()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        var configuration = builder.Configuration;
        var configurationSources = typeof(IConfigurationBuilder).GetProperty("Sources")!.GetValue(configuration) as IList<IConfigurationSource>;

        // 测试默认环境配置
        configurationSources.Should().Contain(u => u is JsonConfigurationSource);
        var jsonConfigurationSource = configurationSources!.FirstOrDefault(u => u is JsonConfigurationSource source && source.Path.EndsWith("furion.json")) as JsonConfigurationSource;

        jsonConfigurationSource.Should().NotBeNull();
    }
}