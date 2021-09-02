// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.ObjectExtensions;
using Furion.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <returns></returns>
    public static IServiceCollection AddAppOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, IAppOptions
    {
        // 创建配置选项
        var optionsBuilder = services.CreateOptionsBuilder<TOptions>(configuration);

        // 添加后期配置
        PostConfigure(optionsBuilder, services);

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

        if (configureOptions != default) optionsBuilder.PostConfigure(configureOptions);

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

        if (configureOptions != default) optionsBuilder.PostConfigure(configureOptions);

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

        if (configureOptions != default) optionsBuilder.PostConfigure(configureOptions);

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

        if (configureOptions != default) optionsBuilder.PostConfigure(configureOptions);

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

        if (configureOptions != default) optionsBuilder.PostConfigure(configureOptions);

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

        if (configureOptions != default) optionsBuilder.PostConfigure(configureOptions);

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

        // 验证选项
        Validate(optionsBuilder, services, appOptionsAttribute);

        return optionsBuilder;
    }

    /// <summary>
    /// 验证选项
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="optionsBuilder">选项类型</param>
    /// <param name="services">服务注册集合</param>
    /// <returns></returns>
    private static OptionsBuilder<TOptions> Validate<TOptions>(OptionsBuilder<TOptions> optionsBuilder, IServiceCollection services, AppOptionsAttribute? appOptionsAttribute)
        where TOptions : class
    {
        var optionsType = typeof(TOptions);

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
        if (appOptionsAttribute?.ValidateOptionsTypes.IsEmpty() == true) return optionsBuilder;

        // 注册所有验证
        foreach (var validateType in appOptionsAttribute?.ValidateOptionsTypes!)
        {
            // 验证类型必须实现 IValidateOptions<TOptions>
            if (!validateOptionsType.IsAssignableFrom(validateType))
                throw new InvalidOperationException($"The value type of `{nameof(AppOptionsAttribute.ValidateOptionsTypes)}` property must implement the `IValidateOptions<TOptions>` interface.");

            // 注册 IValidateOptions 复杂验证
            services.TryAddEnumerable(ServiceDescriptor.Singleton(validateOptionsType, validateType));
        }

        return optionsBuilder;
    }

    /// <summary>
    /// 选项后期配置
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="optionsBuilder">选项类型</param>
    /// <param name="services">服务注册集合</param>
    /// <returns>OptionsBuilder</returns>
    private static OptionsBuilder<TOptions>? PostConfigure<TOptions>(OptionsBuilder<TOptions> optionsBuilder, IServiceCollection services)
        where TOptions : class, IAppOptions
    {
        var optionsType = typeof(TOptions);

        // 扫描后期配置方法
        var postConfigureMethods = optionsType.GetTypeInfo().DeclaredMethods
                                                            .Where(m => (m.Name == nameof(IAppOptions<TOptions>.PostConfigure) || m.Name.EndsWith($".{nameof(IAppOptions<TOptions>.PostConfigure)}"))
                                                                && m.GetParameters()[0].ParameterType == optionsType);

        // 限制选项多次实现 IAppOptionsDependency 接口
        if (postConfigureMethods.Count() > 1)
            throw new InvalidOperationException($"Please ensure that the option class `{optionsType.Name}` has and uniquely implements the `{nameof(IAppOptions)}` interface.");

        var postConfigureMethod = postConfigureMethods.First();

        //  获取后缀选项参数
        var parameterTypes = postConfigureMethod.GetParameters().Select(p => p.ParameterType).ToArray();

        // 创建相同签名委托
        var actionGenericParameterType = typeof(Action).Assembly.GetType($"{typeof(Action).FullName}`{parameterTypes.Length}")
                                            ?.MakeGenericType(parameterTypes);
        var configureOptions = postConfigureMethod.CreateDelegate(actionGenericParameterType!, default(TOptions));

        // 添加选项后期配置
        services.Add(ServiceDescriptor.Describe(typeof(IPostConfigureOptions<TOptions>), sp =>
        {
            sp = sp.Resolve();

            // 添加参数
            var args = new List<object>(parameterTypes.Length + 1) { optionsBuilder.Name };
            args.AddRange(parameterTypes.Skip(1).Select(u => sp.GetRequiredService(u)));
            args.Add(configureOptions);

            // 动态创建 PostConfigureOptions<TOptions, TDep1..TDep5> 对象
            var postConfigureOptionsType = typeof(PostConfigureOptions<TOptions>);
            var runtimePostConfigureOptionsType = postConfigureOptionsType.Assembly.GetType($"{postConfigureOptionsType.Namespace}.{postConfigureOptionsType.Name![..^1]}{parameterTypes.Length}")
                                                            ?.MakeGenericType(parameterTypes);
            var postConfigureOptions = Activator.CreateInstance(runtimePostConfigureOptionsType!, args.ToArray());

            return postConfigureOptions!;
        }, parameterTypes.Length > 1 ? ServiceLifetime.Transient : ServiceLifetime.Singleton));

        return optionsBuilder;
    }
}