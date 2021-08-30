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
    /// <param name="configurationSection">App 配置节点</param>
    /// <param name="configureOptions">AppSettings 后置配置</param>
    /// <returns></returns>
    public static IServiceCollection AddApp(this IServiceCollection services, IConfigurationSection configurationSection, Action<AppSettingsOptions>? configureOptions = default)
    {
        // 注册 App 全局应用配置
        services.AddAppOptions(configurationSection, configureOptions);

        // 注册为单例
        services.AddSingleton<IApp, App>();

        return services;
    }
}