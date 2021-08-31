using Microsoft.Extensions.Configuration;
using System.Diagnostics;

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
    /// <returns></returns>
    public static IHostBuilder AddAppConfiguration(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        hostBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) => configurationBuilder.Configure(configuration));

        Trace.WriteLine(string.Join(";\n", configuration.AsEnumerable().Select(c => $"{c.Key}={c.Value}")));

        return hostBuilder;
    }
}