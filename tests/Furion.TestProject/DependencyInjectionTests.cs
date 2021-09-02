using Furion.TestProject.Filters;
using Furion.TestProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
    private readonly IServiceProvider _appServiceProvider;
    private readonly IAutowriedService _autowriedService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="app"></param>
    public DependencyInjectionTests(IApp app
        , IServiceProvider serviceProvider
        , IAutowriedService autowriedService)
    {
        _app = app;
        _serviceProvider = serviceProvider;
        _appServiceProvider = serviceProvider.Resolve();
        _autowriedService = autowriedService;
    }

    [AutowiredServices]
    IApp? App { get; set; }

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
            && _app.Equals(_appServiceProvider.GetRequiredService<IApp>())
            && _app.Equals(_autowriedService.App);
    }
}