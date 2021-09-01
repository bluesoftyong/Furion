using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 配置主机构建器拓展类
/// </summary>
public static class ConfigurationHostBuilderExtensions
{
    /// <summary>
    /// 添加框架初始配置
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <returns></returns>
    public static IHostBuilder AddAppConfiguration(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) => configurationBuilder.Configure(hostingContext.Configuration, hostingContext.HostingEnvironment));

        return hostBuilder;
    }
}