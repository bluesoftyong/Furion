using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 配置 Furion 框架初始化
/// </summary>
public static class FurionHostBuilderExtensions
{
    /// <summary>
    /// 初始化框架服务
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <returns></returns>
    public static IHostBuilder UseFurion(this IHostBuilder hostBuilder)
    {
        // 添加框架初始配置
        hostBuilder.AddAppConfiguration();

        // 配置初始服务
        hostBuilder.ConfigureServices((hostBuilderContext, services) =>
        {
            // 注册 HttpContext 访问器
            services.AddHttpContextAccessor();

            // 注册 App 全局应用对象服务
            services.AddApp(hostBuilderContext.Configuration);
        });

        return hostBuilder;
    }
}