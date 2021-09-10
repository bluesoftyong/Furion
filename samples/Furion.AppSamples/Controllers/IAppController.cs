using Microsoft.AspNetCore.Mvc;

namespace Furion.AppSamples.Controllers;

/// <summary>
/// App 模块 IApp 服务使用示例
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class IAppController : ControllerBase
{
    private readonly IApp _app;
    public IAppController(IApp app)
    {
        _app = app;
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetConfiguration()
    {
        return $"默认日志级别：{_app.Configuration["Logging:LogLevel:Default"]}";
    }

    /// <summary>
    /// 获取环境信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetEnvironmentInfo()
    {
        return $"当前环境名称：{_app.Environment.EnvironmentName}，是否开发环境：{_app.Environment.IsDevelopment()}，启动目录：{_app.Environment.ContentRootPath}";
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetServiceByHostServices()
    {
        return $"当前服务：{_app.Host.Services.GetService<IApp>()}";
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetServiceByServiceProvider()
    {
        return $"当前服务：{_app.ServiceProvider.GetService<IServiceProvider>()}";
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetService()
    {
        // GetRequiredService 同下
        return $"当前服务：{_app.GetService<IApp>()}";
    }

    /// <summary>
    /// 解析不为空服务
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GetRequiredService()
    {
        // GetService 同下
        return $"当前服务：{_app.GetRequiredService(typeof(IApp))}";
    }
}