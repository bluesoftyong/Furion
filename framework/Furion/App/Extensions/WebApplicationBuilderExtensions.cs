using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    /// <returns></returns>
    public static WebApplicationBuilder Inject(this WebApplicationBuilder webApplicationBuilder)
    {
        var services = webApplicationBuilder.Services;
        var configuration = webApplicationBuilder.Configuration;
        var environment = webApplicationBuilder.Environment;

        // 添加初始配置
        configuration.AddAppConfiguration(environment);

        // 注册 HttpContext 访问器
        services.AddHttpContextAccessor();

        // 注册 App 全局应用对象服务
        services.AddApp(configuration);

        return webApplicationBuilder;
    }
}