using Microsoft.AspNetCore.Mvc;

namespace Furion.Samples.AppSamples;

/// <summary>
/// App 模块 IApp 服务使用示例
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class IAppSamplesController : ControllerBase
{
    private readonly IApp _app;
    public IAppSamplesController(IApp app)
    {
        _app = app;
    }

    [HttpPost]
    public void Tests()
    {
        // 解析服务
        var app = _app.ServiceProvider.GetRequiredService<IApp>();
        Console.WriteLine(_app == app);

        // 读取配置
        var environmentVariablesPrefix = _app.Configuration["AppSettings:EnvironmentVariablesPrefix"];
        Console.WriteLine(environmentVariablesPrefix);

        // 判断环境
        var isDevelopment = _app.Environment.IsDevelopment();
        Console.WriteLine(isDevelopment);

        // 通过主机对象服务解析服务
        var app1 = _app.Host.Services.GetRequiredService<IApp>();
        Console.WriteLine(app1);

        // 直接解析服务
        var notRegisterService = _app.GetService<INotRegisterService>();
        var registerService = _app.GetRequiredService(typeof(IRegisterService));
        Console.WriteLine(notRegisterService);
        Console.WriteLine(registerService);
    }
}