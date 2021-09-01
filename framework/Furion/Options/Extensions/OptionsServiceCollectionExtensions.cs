using Furion.ObjectExtensions;
using Furion.Options;
using Furion.Options.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, IAppOptionsDependency
    {
        // 创建配置选项
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        // 添加后期配置
        _ = optionsBuilder.PostConfigure();

        return services;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">后期配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, Action<TOptions>? configureOptions = default)
        where TOptions : class
    {
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        if (configureOptions != default) _ = optionsBuilder.PostConfigure(configureOptions);

        return services;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <typeparam name="TDep">依赖服务</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">后期配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions, TDep>(this IServiceCollection services, IConfiguration configuration, Action<TOptions, TDep>? configureOptions = default)
        where TOptions : class
        where TDep : class
    {
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        if (configureOptions != default) _ = optionsBuilder.PostConfigure(configureOptions);

        return services;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <typeparam name="TDep1">依赖服务</typeparam>
    /// <typeparam name="TDep2">依赖服务</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">后期配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions, TDep1, TDep2>(this IServiceCollection services, IConfiguration configuration, Action<TOptions, TDep1, TDep2>? configureOptions = default)
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
    {
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        if (configureOptions != default) _ = optionsBuilder.PostConfigure(configureOptions);

        return services;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <typeparam name="TDep1">依赖服务</typeparam>
    /// <typeparam name="TDep2">依赖服务</typeparam>
    /// <typeparam name="TDep3">依赖服务</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">后期配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions, TDep1, TDep2, TDep3>(this IServiceCollection services, IConfiguration configuration, Action<TOptions, TDep1, TDep2, TDep3>? configureOptions = default)
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
        where TDep3 : class
    {
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        if (configureOptions != default) _ = optionsBuilder.PostConfigure(configureOptions);

        return services;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <typeparam name="TDep1">依赖服务</typeparam>
    /// <typeparam name="TDep2">依赖服务</typeparam>
    /// <typeparam name="TDep3">依赖服务</typeparam>
    /// <typeparam name="TDep4">依赖服务</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">后期配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions, TDep1, TDep2, TDep3, TDep4>(this IServiceCollection services, IConfiguration configuration, Action<TOptions, TDep1, TDep2, TDep3, TDep4>? configureOptions = default)
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
        where TDep3 : class
        where TDep4 : class
    {
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        if (configureOptions != default) _ = optionsBuilder.PostConfigure(configureOptions);

        return services;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <typeparam name="TDep1">依赖服务</typeparam>
    /// <typeparam name="TDep2">依赖服务</typeparam>
    /// <typeparam name="TDep3">依赖服务</typeparam>
    /// <typeparam name="TDep4">依赖服务</typeparam>
    /// <typeparam name="TDep5">依赖服务</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">后期配置</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>(this IServiceCollection services, IConfiguration configuration, Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>? configureOptions = default)
        where TOptions : class
        where TDep1 : class
        where TDep2 : class
        where TDep3 : class
        where TDep4 : class
        where TDep5 : class
    {
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        if (configureOptions != default) _ = optionsBuilder.PostConfigure(configureOptions);

        return services;
    }

    /// <summary>
    /// 创建选项构建器
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <returns></returns>
    private static OptionsBuilder<TOptions> CreateOptionsBuilder<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        var optionsType = typeof(TOptions);
        var appOptionsAttribute = optionsType.GetTypeAttribute<AppOptionsAttribute>();

        // 解析配置类型（自动识别是否是节点配置对象）
        var configurationSection = configuration is IConfigurationSection section
                                    ? section
                                    : configuration.GetSection(
                                          string.IsNullOrWhiteSpace(appOptionsAttribute?.SectionKey)
                                              ? optionsType.Name.SubSuffix("Options")
                                              : appOptionsAttribute.SectionKey
                                       );

        // 注册选项
        var optionsBuilder = services.AddOptions<TOptions>()
                   .Bind(configurationSection, binderOptions =>
                   {
                       binderOptions.ErrorOnUnknownConfiguration = appOptionsAttribute?.ErrorOnUnknownConfiguration ?? false;
                       binderOptions.BindNonPublicProperties = appOptionsAttribute?.BindNonPublicProperties ?? false;
                   });

        // 如果未明确关闭数据验证，则默认启用
        if (appOptionsAttribute?.ValidateDataAnnotations == false) return optionsBuilder;

        // 启用特性验证
        optionsBuilder.ValidateDataAnnotations();

        // 配置复杂验证
        var validateOptionsType = typeof(IValidateOptions<TOptions>);

        // 禁止选项自实现 IValidateOptions<TOptions> 类型
        if (validateOptionsType.IsAssignableFrom(optionsType))
            throw new InvalidOperationException($"Type `{optionsType.Name}` prohibits the implementation of `IValidateOptions<TOptions>`.");

        // 配置复杂验证支持
        if (appOptionsAttribute?.ValidateOptionsTypes == default || appOptionsAttribute.ValidateOptionsTypes.Length == 0) return optionsBuilder;

        // 注册所有验证
        foreach (var validateType in appOptionsAttribute.ValidateOptionsTypes)
        {
            // 验证类型必须实现 IValidateOptions<TOptions>
            if (!validateOptionsType.IsAssignableFrom(validateType))
                throw new InvalidOperationException($"The value type of `{nameof(AppOptionsAttribute.ValidateOptionsTypes)}` property must implement the `IValidateOptions<TOptions>` interface.");

            // 注册 IValidateOptions 复杂验证
            services.TryAddEnumerable(ServiceDescriptor.Singleton(validateOptionsType, validateType));
        }

        return optionsBuilder;
    }
}