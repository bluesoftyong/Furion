using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// 配置对象 WebApplication 拓展类
/// </summary>
public static class ConfigurationWebApplicationBuilderExtensions
{
    /// <summary>
    /// 添加框架初始配置
    /// </summary>
    /// <param name="webApplicationBuilder">WebApplication 构建器</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddAppConfiguration(this WebApplicationBuilder webApplicationBuilder)
    {
        var configuration = webApplicationBuilder.Configuration as IConfiguration;
        var configurationBuilder = webApplicationBuilder.Configuration as IConfigurationBuilder;
        var environment = webApplicationBuilder.Environment;

        configurationBuilder.Configure(configuration, environment);

        return webApplicationBuilder;
    }
}