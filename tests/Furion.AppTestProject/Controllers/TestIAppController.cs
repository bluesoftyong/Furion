using FluentAssertions;
using Furion.AppTestProject.Dependencies;
using Microsoft.AspNetCore.Mvc;

namespace Furion.AppTestProject.Controllers;

/// <summary>
/// 测试 App 模块 IApp 服务
/// </summary>
[Route("[controller]/[action]")]
[ApiController]
public class TestIAppController : ControllerBase
{
    private readonly IApp _app;
    private readonly IApp _app2;
    private IServiceProvider _serviceProvider;
    private IHost _host;

    public TestIAppController(IApp app
        , IApp app2
        , IServiceProvider serviceProvider
        , IHost host)
    {
        _app = app;
        _app2 = app2;
        _serviceProvider = serviceProvider;
        _host = host;
    }

    /// <summary>
    /// 测试 IApp 是否是单例
    /// </summary>
    /// <param name="app3"></param>
    [HttpPost]
    public void TestIAppIsSingleton([FromServices] IApp app3)
    {
        var app4 = _serviceProvider.GetService<IApp>();
        var app5 = _host.Services.GetService<IApp>();

        // 测试非空
        app3.Should().NotBeNull();
        app4.Should().NotBeNull();
        app5.Should().NotBeNull();

        // 测试是否同一个实例
        _app.Should().BeSameAs(_app2)
            .And.BeSameAs(app3)
            .And.BeSameAs(app4)
            .And.BeSameAs(app5);
    }

    /// <summary>
    /// 测试 IApp 实例
    /// </summary>
    [HttpPost]
    public void TestIAppNotPublicInstance()
    {
        var type = _app.GetType();

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
    [HttpPost]
    public void TestIAppProperties()
    {
        var type = _app.GetType();

        // 测试非空及是否实现特定接口
        _app.ServiceProvider.Should().NotBeNull()
            .And.BeAssignableTo<IServiceProvider>();

        // 测试非空及是否实现特定接口
        _app.Configuration.Should().NotBeNull()
            .And.BeAssignableTo<IConfiguration>();

        // 测试非空及是否实现特定接口
        _app.Environment.Should().NotBeNull()
            .And.BeAssignableTo<IHostEnvironment>();

        // 测试非空及是否实现特定接口
        _app.Host.Should().NotBeNull()
            .And.BeAssignableTo<IHost>();

        // 测试属性只读
        type.Properties().Should().NotBeWritable();
    }

    /// <summary>
    /// 测试 IApp 方法
    /// </summary>
    [HttpPost]
    public void TestIAppMethods()
    {
        var type = _app.GetType();

        // 测试解析服务
        _app.GetService<IHost>().Should().NotBeNull();
        _app.GetService(typeof(IHost)).Should().NotBeNull();
        _app.GetService<INotRegisterService>().Should().BeNull();
        _app.GetService(typeof(INotRegisterService)).Should().BeNull();

        // 测试必选服务
        _app.GetRequiredService<IHost>().Should().NotBeNull();
        _app.GetRequiredService(typeof(IHost)).Should().NotBeNull();

        // 测试解析未注册服务
        _app.Invoking(u => u.GetRequiredService<INotRegisterService>()).Should().Throw<InvalidOperationException>();
        _app.Invoking(u => u.GetRequiredService(typeof(INotRegisterService))).Should().Throw<InvalidOperationException>();

        // 测试解析非 Class 类型
        _app.Invoking(u => u.GetService(typeof(string))).Should().NotThrow();
        _app.Invoking(u => u.GetRequiredService(typeof(string))).Should().Throw<InvalidOperationException>();
    }
}