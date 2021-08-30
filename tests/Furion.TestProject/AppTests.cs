using Microsoft.AspNetCore.Mvc;

namespace Furion.TestProject.Controllers;

/// <summary>
/// App 全局应用对象集成测试 RESTful Api
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class AppTests : ControllerBase
{
    private readonly IApp _app;
    private readonly IApp _app2;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHost _host;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="app"></param>
    /// <param name="app2"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="host"></param>
    /// <param name="httpContextAccessor"></param>
    public AppTests(IApp app
        , IApp app2
        , IServiceProvider serviceProvider
        , IHost host
        , IHttpContextAccessor httpContextAccessor)
    {
        _app = app;
        _app2 = app2;
        _serviceProvider = serviceProvider;
        _host = host;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 测试 IApp 是否是单例
    /// </summary>
    /// <param name="app3"></param>
    /// <returns></returns>
    [HttpPost]
    public bool TestIsSingleton([FromServices] IApp app3)
    {
        return _app.Equals(_app2)
            && _app.Equals(_serviceProvider.GetRequiredService<IApp>())
            && _app.Equals(_host.Services.GetRequiredService<IApp>())
            && _app.Equals(app3)
            && _app.Equals(_httpContextAccessor.HttpContext?.RequestServices?.GetRequiredService<IApp>());
    }

    /// <summary>
    /// 测试 AppSettings 不为 Null
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public bool TestAppSettingsNotNull()
    {
        return _app.AppSettings != default;
    }
}