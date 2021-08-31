using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 配置主机构建器拓展类
/// </summary>
public static class ConfigurationHostBuilderExtensions
{
    /// <summary>
    /// 添加应用配置
    /// </summary>
    /// <remarks>包含自动扫描、环境配置</remarks>
    /// <param name="hostBuilder">主机构建器</param>
    /// <param name="configuration">默认启动配置对象</param>
    /// <param name="environment">环境对象</param>
    /// <returns></returns>
    public static IHostBuilder AddAppConfiguration(this IHostBuilder hostBuilder, IConfiguration configuration, IHostEnvironment environment)
    {
        hostBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) => configurationBuilder.Configure(configuration, environment));

        return hostBuilder;
    }
}