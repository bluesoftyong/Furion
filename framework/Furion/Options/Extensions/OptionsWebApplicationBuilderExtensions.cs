using Furion.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// 选项 WebApplication 拓展类
/// </summary>
public static class OptionsWebApplicationBuilderExtensions
{
    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="webApplicationBuilder">WebApplication 构建器</param>
    /// <returns></returns>
    public static WebApplicationBuilder AddAppOptions<TOptions>(this WebApplicationBuilder webApplicationBuilder)
        where TOptions : class, IAppOptions<TOptions>
    {
        var services = webApplicationBuilder.Services;
        var configuration = webApplicationBuilder.Configuration;

        services.AddAppOptions<TOptions>(configuration);

        return webApplicationBuilder;
    }
}