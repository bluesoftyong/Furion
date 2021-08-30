using Furion;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App 全局应用服务拓展类
/// </summary>
public static class AppServiceCollectionExtensions
{
    /// <summary>
    /// 添加 App 全局应用服务
    /// </summary>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">AppSettings 后置配置</param>
    /// <returns></returns>
    public static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册 App 全局应用配置
        services.AddAppOptions<AppSettingsOptions>(configuration);

        // 注册为单例
        services.AddSingleton<IApp, App>();

        return services;
    }
}