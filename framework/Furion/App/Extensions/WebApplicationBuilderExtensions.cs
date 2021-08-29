using Furion;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// WebApplication 拓展类
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// 初始化框架服务
    /// </summary>
    /// <param name="webApplicationBuilder">WebApplication 构建器</param>
    /// <param name="appSettingsSection">AppSettings 配置节点</param>
    /// <param name="appSettingsConfigureOptions">AppSettings 后置配置</param>
    /// <returns></returns>
    public static WebApplicationBuilder Inject(this WebApplicationBuilder webApplicationBuilder, string? appSettingsSection = default, Action<AppSettingsOptions>? appSettingsConfigureOptions = default)
    {
        // 注册 App 全局应用对象服务
        webApplicationBuilder.Services.AddApp(
            webApplicationBuilder.Configuration.GetSection(appSettingsSection ?? "AppSettings")
            , appSettingsConfigureOptions);

        return webApplicationBuilder;
    }
}