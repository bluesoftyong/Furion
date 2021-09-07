using Furion.TestProject.Filters;
using Furion.TestProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace Furion.TestProject.Controllers;

/// <summary>
/// 依赖注入集成测试 RESTful Api
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class DependencyInjectionTests : ControllerBase
{
    private readonly IApp _app;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProvider _wrapServiceProvider;
    private readonly IAppServiceProvider _appServiceProvider;
    private readonly IAutowriedService _autowriedService;
    private readonly INamedServiceProvider _namedServiceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="app"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="appServiceProvider"></param>
    /// <param name="autowriedService"></param>
    /// <param name="namedServiceProvider"></param>
    public DependencyInjectionTests(IApp app
        , IServiceProvider serviceProvider
        , IAppServiceProvider appServiceProvider
        , IAutowriedService autowriedService
        , INamedServiceProvider namedServiceProvider)
    {
        _app = app;
        _serviceProvider = serviceProvider;
        _appServiceProvider = appServiceProvider;
        _wrapServiceProvider = serviceProvider.CreateProxy();
        _autowriedService = autowriedService;
        _namedServiceProvider = namedServiceProvider;
    }

    [AutowiredServices]
    IApp? App { get; }

    /// <summary>
    /// 测试构造函数注入、属性注入、方法注入、手动解析
    /// </summary>
    /// <returns></returns>
    [HttpPost, ServiceFilter(typeof(TestControllerFilter))]
    public bool TestService([FromServices] IApp app2)
    {
        return _app.Equals(App)
            && _app.Equals(app2)
            && _app.Equals(_serviceProvider.GetRequiredService<IApp>())
            && _app.Equals(_wrapServiceProvider.GetRequiredService<IApp>())
            && _app.Equals(_appServiceProvider.GetRequiredService<IApp>())
            && _app.Equals(_autowriedService.App)
            && App != null;
    }

    /// <summary>
    /// 测试命名服务
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public string[] TestNamedService()
    {
        return new[] {
            _namedServiceProvider.GetRequiredService<ITestNamedService>("test1").GetType().Name,
            _namedServiceProvider.GetRequiredService<ITestNamedService>("test2").GetType().Name,
            _namedServiceProvider.GetRequiredService<ITestNamedService>("test3").GetType().Name,
            _namedServiceProvider.GetRequiredService<ITestNamedService>("test4").GetType().Name
        };
    }

    /// <summary>
    /// 测试扫描依赖注入服务
    /// </summary>
    /// <param name="testDependencyService"></param>
    /// <param name="testDependencyService1"></param>
    /// <returns></returns>
    [HttpPost]
    public bool TestDependencyService([FromServices] ITestDependencyService testDependencyService, [FromServices] ITest2DependencyService testDependencyService1)
    {
        return testDependencyService != null
            && testDependencyService1 != null;
    }

    /// <summary>
    /// 测试扫描依赖注入服务
    /// </summary>
    /// <param name="testDependencyService"></param>
    /// <param name="testDependencyService1"></param>
    /// <returns></returns>
    [HttpPost]
    public bool TestDependencyService2([FromServices] TestDependencyService testDependencyService1)
    {
        return testDependencyService1 != null;
    }

    /// <summary>
    /// 测试泛型接口
    /// </summary>
    /// <param name="genericDependencyService"></param>
    /// <returns></returns>
    [HttpPost]
    public bool TestGenericDependencyService([FromServices] ITestGenericDependencyService<int> genericDependencyService)
    {
        return genericDependencyService != null;
    }

    /// <summary>
    /// 测试工厂解析服务
    /// </summary>
    /// <param name="test3DependencyService"></param>
    /// <returns></returns>
    [HttpPost]
    public bool TestFactoryService([FromServices] ITest3DependencyService test3DependencyService)
    {
        return test3DependencyService.NoNull();
    }
}