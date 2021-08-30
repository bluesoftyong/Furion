using Furion.ObjectExtensions;
using Furion.Options;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 选项服务拓展类
/// </summary>
public static class OptionsServiceCollectionExtensions
{
    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, IAppOptions<TOptions>
    {
        var optionsType = typeof(TOptions);
        var appOptionsAttribute = optionsType.GetTypeAttribute<AppOptionsAttribute>();

        // 获取配置 key
        var sectionKey = string.IsNullOrWhiteSpace(appOptionsAttribute?.SectionKey)
                            ? optionsType.Name.SubSuffix("Options")
                            : appOptionsAttribute.SectionKey;

        // 读取后缀配置
        var postConfigureMethod = optionsType.GetTypeInfo().DeclaredMethods.First(m => m.Name == nameof(IAppOptions<TOptions>.PostConfigure));

        // 注册服务
        services.AddAppOptions<TOptions>(configuration.GetSection(sectionKey), options => postConfigureMethod.Invoke(options, new[] { options }));

        return services;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configurationSection">配置节点对象</param>
    /// <param name="configureOptions">后置配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions>(this IServiceCollection services, IConfigurationSection configurationSection, Action<TOptions>? configureOptions = default)
        where TOptions : class
    {
        // 注册选项
        var optionsBuilder = services.AddOptions<TOptions>()
                   .Bind(configurationSection, binderOptions =>
                   {
                       binderOptions.ErrorOnUnknownConfiguration = true;
                       binderOptions.BindNonPublicProperties = true;
                   })
                   .ValidateDataAnnotations();

        if (configureOptions != default) _ = optionsBuilder.PostConfigure(configureOptions);

        return services;
    }
}