using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Furion.UnitTests;

/// <summary>
/// IApp 服务测试
/// </summary>
public class IAppTests
{
    /// <summary>
    /// 测试 IApp 是否是单例
    /// </summary>
    /// <param name="app3"></param>
    [Fact]
    public void TestSingletonLifetime()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        using var app = builder.Build();
        var services = app.Services;

        var app1 = services.GetService<IApp>();
        var app2 = services.GetService<IApp>();
        var app3 = services.GetRequiredService<IServiceProvider>().GetService<IApp>();
        var app4 = services.GetRequiredService<IHost>().Services.GetService<IApp>();

        // 测试非空
        app1.Should().NotBeNull();
        app2.Should().NotBeNull();
        app3.Should().NotBeNull();
        app4.Should().NotBeNull();

        // 测试是否同一个实例
        app1.Should().BeSameAs(app2)
            .And.BeSameAs(app3)
            .And.BeSameAs(app4);
    }

    /// <summary>
    /// 测试 IApp 实例
    /// </summary>
    [Fact]
    public void TestInstance()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        using var app = builder.Build();
        var services = app.Services;

        var app1 = services.GetRequiredService<IApp>();
        var type = app1.GetType();

        // 测试密封类
        type.Should().BeSealed();

        // 测试非公开
        type.IsPublic.Should().BeFalse();

        // 测试实现类命名空间
        type.Namespace.Should().Equals("Furion");
    }

    /// <summary>
    /// 测试 IApp 属性
    /// </summary>
    [Fact]
    public void TestProperties()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        using var app = builder.Build();
        var services = app.Services;

        var app1 = services.GetRequiredService<IApp>();
        var type = app1.GetType();

        // 测试非空及是否实现特定接口
        app1.ServiceProvider.Should().NotBeNull()
            .And.BeAssignableTo<IServiceProvider>();

        // 测试非空及是否实现特定接口
        app1.Configuration.Should().NotBeNull()
            .And.BeAssignableTo<IConfiguration>();

        // 测试非空及是否实现特定接口
        app1.Environment.Should().NotBeNull()
            .And.BeAssignableTo<IHostEnvironment>();

        // 测试非空及是否实现特定接口
        app1.Host.Should().NotBeNull()
            .And.BeAssignableTo<IHost>();

        // 测试属性只读
        type.Properties().Should().NotBeWritable();
    }

    /// <summary>
    /// 测试 IApp 方法
    /// </summary>
    [Fact]
    public void TestMethods()
    {
        var builder = WebApplication.CreateBuilder().UseFurion();
        using var app = builder.Build();
        var services = app.Services;

        var app1 = services.GetRequiredService<IApp>();
        var type = app1.GetType();

        // 测试解析服务
        app1.GetService<IHost>().Should().NotBeNull();
        app1.GetService(typeof(IHost)).Should().NotBeNull();
        app1.GetService<INotRegisterService>().Should().BeNull();
        app1.GetService(typeof(INotRegisterService)).Should().BeNull();

        // 测试必选服务
        app1.GetRequiredService<IHost>().Should().NotBeNull();
        app1.GetRequiredService(typeof(IHost)).Should().NotBeNull();

        // 测试解析未注册服务
        app1.Invoking(u => u.GetRequiredService<INotRegisterService>()).Should().Throw<InvalidOperationException>();
        app1.Invoking(u => u.GetRequiredService(typeof(INotRegisterService))).Should().Throw<InvalidOperationException>();

        // 测试解析非 Class 类型
        app1.Invoking(u => u.GetService(typeof(string))).Should().NotThrow();
        app1.Invoking(u => u.GetRequiredService(typeof(string))).Should().Throw<InvalidOperationException>();
    }
}