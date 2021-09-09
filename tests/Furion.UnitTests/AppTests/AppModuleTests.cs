using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
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
        services.Invoking(s => s.GetRequiredService<IOptions<AppSettingsOptions>>().Value).Should().NotThrow();
    }

    /// <summary>
    /// 测试手动注册
    /// </summary>
    [Fact]
    public void TestManualRegister()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddApp(builder.Configuration);
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
}