using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Furion;

/// <summary>
/// App 全局应用对象接口规范
/// </summary>
public interface IApp
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 配置对象
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// App 全局配置
    /// </summary>
    AppSettingsOptions AppSettings { get; }

    /// <summary>
    /// 主机环境
    /// </summary>
    IHostEnvironment Environment { get; }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或Null </returns>
    TService? GetService<TService>()
       where TService : class;

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或异常 </returns>
    TService? GetRequiredService<TService>()
       where TService : class;
}