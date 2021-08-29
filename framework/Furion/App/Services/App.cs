using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Furion;

/// <summary>
/// App 全局应用对象实现类
/// </summary>
public sealed partial class App : IApp
{
    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<App> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="optionsMonitor">配置选项</param>
    /// <param name="serviceProvider">服务提供器</param>
    public App(ILogger<App> logger,
        IOptionsMonitor<AppSettingsOptions> optionsMonitor,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        AppSettings = optionsMonitor.CurrentValue;
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// 服务提供器
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// App 全局配置
    /// </summary>
    public AppSettingsOptions AppSettings { get; }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或Null </returns>
    public TService? GetService<TService>()
        where TService : class
    {
        return ServiceProvider.GetService<TService>();
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或异常 </returns>
    public TService? GetRequiredService<TService>()
        where TService : class
    {
        return ServiceProvider.GetRequiredService<TService>();
    }
}