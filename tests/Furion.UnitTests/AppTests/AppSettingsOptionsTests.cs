using FluentAssertions;
using Furion.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Furion.UnitTests;

/// <summary>
/// AppSettingsOptions 测试
/// </summary>
public class AppSettingsOptionsTests
{
    private readonly ITestOutputHelper _output;
    public AppSettingsOptionsTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// 测试 AppSettingsOptions 非空
    /// </summary>
    [Fact]
    public async Task TestNotNull()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        builder.WebHost.UseTestServer();
        using var app = builder.Build();
        var services = app.Services;

        app.Run(context =>
        {
            // 测试非空
            var optionsSnapshot = context.RequestServices.GetRequiredService<IOptionsSnapshot<AppSettingsOptions>>();
            optionsSnapshot.Value.Should().NotBeNull();
            return Task.CompletedTask;
        });

        await app.StartAsync();

        var client = app.GetTestClient();
        var result = await client.GetAsync("http://localhost/");
        result.EnsureSuccessStatusCode();

        var options = services.GetRequiredService<IOptions<AppSettingsOptions>>();
        var optionsMonitor = services.GetRequiredService<IOptionsMonitor<AppSettingsOptions>>();

        // 测试非空
        options.Value.Should().NotBeNull();
        optionsMonitor.CurrentValue.Should().NotBeNull();

    }

    /// <summary>
    /// 测试 AppSettingsOptions 特性
    /// </summary>
    [Fact]
    public void TestAttributes()
    {
        var type = typeof(AppSettingsOptions);

        // 测试是否声明 [AppOptions] 特性
        type.Should().BeDecoratedWith<AppOptionsAttribute>();

        // 测试 [AppOptions] 特性值
        var appOptionsAttribute = type.GetCustomAttribute<AppOptionsAttribute>(false);
        appOptionsAttribute.Should().NotBeNull()
            .And.Match<AppOptionsAttribute>(u => !string.IsNullOrWhiteSpace(u.SectionKey) && u.SectionKey.Equals("AppSettings"))
            .And.Match<AppOptionsAttribute>(u => u.ErrorOnUnknownConfiguration);
    }

    /// <summary>
    /// 测试 AppSettingsOptions 接口
    /// </summary>
    [Fact]
    public void TestInterfaces()
    {
        var type = typeof(AppSettingsOptions);
        var interfaces = type.GetInterfaces();

        // 测试实现 IAppOptions<AppSettingsOptions>
        interfaces.Should().Contain(u => u == typeof(IAppOptions<AppSettingsOptions>));
    }

    /// <summary>
    /// 测试 AppSettingsOptions 缺省配置
    /// </summary>
    [Fact]
    public void TestDefaultValue()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.Sources.Clear();
        });

        using var app = builder.Build();
        var services = app.Services;
        var configuration = services.GetRequiredService<IConfiguration>();
        var appSettingsOptions = services.GetRequiredService<IOptionsMonitor<AppSettingsOptions>>().CurrentValue;

        // 测试缺省
        configuration.GetSection("AppSettings").Exists().Should().BeFalse();

        // 测试属性默认值
        appSettingsOptions.Should().NotBeNull()
            .And.Match<AppSettingsOptions>(u => u.EnvironmentVariablesPrefix!.Equals("FURION_"))
            .And.Match<AppSettingsOptions>(u => u.CustomizeConfigurationFiles == null);
    }

    /// <summary>
    /// 测试 AppSettingsOptions 自定义配置
    /// </summary>
    [Fact]
    public void TestCustomizeConfiguration()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();

        using var app = builder.Build();
        var services = app.Services;
        var configuration = services.GetRequiredService<IConfiguration>();
        var appSettingsOptions = services.GetRequiredService<IOptionsMonitor<AppSettingsOptions>>().CurrentValue;

        // 测试存在
        configuration.GetSection("AppSettings").Exists().Should().BeTrue();

        // 测试属性设置值
        appSettingsOptions.Should().NotBeNull()
            .And.Match<AppSettingsOptions>(u => u.EnvironmentVariablesPrefix!.Equals("MY_FURION_"))
            .And.Match<AppSettingsOptions>(u => u.CustomizeConfigurationFiles != null && u.CustomizeConfigurationFiles.Length > 0);
    }
}