// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.Extensions.ObjectUtilities;
using Furion.Helpers.ObjectUtilities;
using Furion.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Extensions.Options;

/// <summary>
/// OptionsBuilder 拓展类
/// </summary>
public static class OptionsBuilderExtensions
{
    /// <summary>
    /// 配置选项构建器
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="optionsBuilder">选项构建器实例</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>OptionsBuilder{TOptions}</returns>
    public static OptionsBuilder<TOptions> ConfigureBuilder<TOptions>(this OptionsBuilder<TOptions> optionsBuilder, IConfiguration configuration)
        where TOptions : class
    {
        // 绑定选项配置
        optionsBuilder.ConfigureDefaults(configuration).ConfigureBuilder();

        return optionsBuilder;
    }

    /// <summary>
    /// 配置选项构建器
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="optionsBuilder">选项构建器实例</param>
    /// <returns>OptionsBuilder{TOptions}</returns>
    public static OptionsBuilder<TOptions> ConfigureBuilder<TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        var optionsType = typeof(TOptions);
        var optionsBuilderDependency = typeof(IOptionsBuilder<TOptions>);

        // 获取所有构建器依赖接口
        var builderInterfaces = optionsType.GetInterfaces().Where(u => optionsBuilderDependency.IsAssignableFrom(u) && u != optionsBuilderDependency);
        if (!builderInterfaces.Any())
        {
            return optionsBuilder;
        }

        // 循环调用
        foreach (var builderInterface in builderInterfaces)
        {
            InvokeMapMethod(optionsBuilder, optionsType, builderInterface);
        }

        return optionsBuilder;
    }

    /// <summary>
    /// 配置选项常规默认处理
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="optionsBuilder">选项构建器实例</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>OptionsBuilder{TOptions}</returns>
    public static OptionsBuilder<TOptions> ConfigureDefaults<TOptions>(this OptionsBuilder<TOptions> optionsBuilder, IConfiguration configuration)
        where TOptions : class
    {
        // 获取 [OptionsBuilder] 特性
        var optionsType = typeof(TOptions);
        var optionsBuilderAttribute = typeof(TOptions).GetTypeAttribute<OptionsBuilderAttribute>();

        // 解析配置类型（自动识别是否是节点配置对象）
        var configurationSection = configuration is IConfigurationSection section
                                    ? section
                                    : configuration.GetSection(
                                          string.IsNullOrWhiteSpace(optionsBuilderAttribute?.SectionKey)
                                              ? optionsType.Name.SubSuffix("Options")
                                              : optionsBuilderAttribute.SectionKey
                                       );

        // 绑定配置
        optionsBuilder.Bind(configurationSection, binderOptions =>
        {
            binderOptions.ErrorOnUnknownConfiguration = optionsBuilderAttribute?.ErrorOnUnknownConfiguration ?? false;
            binderOptions.BindNonPublicProperties = optionsBuilderAttribute?.BindNonPublicProperties ?? false;
        });

        // 注册验证特性支持
        if (optionsBuilderAttribute?.ValidateDataAnnotations == true)
        {
            optionsBuilder.ValidateDataAnnotations();
        }

        // 注册复杂验证类型
        if (optionsBuilderAttribute?.ValidateOptionsTypes.IsEmpty() == false)
        {
            var validateOptionsType = typeof(IValidateOptions<TOptions>);

            // 注册复杂选项
            Array.ForEach(optionsBuilderAttribute.ValidateOptionsTypes!, type =>
            {
                optionsBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(validateOptionsType, type));
            });
        }

        return optionsBuilder;
    }

    /// <summary>
    /// 调用 OptionsBuilder{TOptions} 对应方法
    /// </summary>
    /// <param name="optionsBuilder">选项构建器实例</param>
    /// <param name="optionsType">选项类型</param>
    /// <param name="builderInterface">构建器接口</param>
    private static void InvokeMapMethod(object optionsBuilder, Type optionsType, Type builderInterface)
    {
        // 获取接口对应 OptionsBuilder 方法映射信息
        var optionsBuilderMethodMapAttribute = builderInterface.GetCustomAttribute<OptionsBuilderMethodMapAttribute>()!;
        var methodName = optionsBuilderMethodMapAttribute.MethodName;

        // 获取泛型定义参数
        var genericArguments = builderInterface.GetGenericArguments();

        // 创建参数委托类型
        var delegateType = optionsBuilderMethodMapAttribute.VoidReturn
            ? TypeHelpers.CreateActionDelegate(genericArguments)
            : TypeHelpers.CreateFuncDelegate(typeof(bool), genericArguments);

        // 创建方法调用参数
        var bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var argumentDelegate = optionsType.GetMethods(bindingAttr)
                .First(u => u.Name == methodName || u.Name.EndsWith("." + methodName) && u.GetParameters().Length == genericArguments.Length)
                .CreateDelegate(delegateType, default);

        // 创建委托参数表达式
        var argumentExpression = Expression.Parameter(delegateType, "arg0");

        // 创建调用方法表达式
        var callExpression = Expression.Call(Expression.Constant(optionsBuilder)
            , methodName
            , genericArguments.IsEmpty() ? default : genericArguments!.Skip(1).ToArray()
            , new[] { argumentExpression });

        // 创建调用委托
        var @delegate = Expression.Lambda(callExpression, argumentExpression).Compile();

        // 动态调用方法
        @delegate.DynamicInvoke(argumentDelegate);
    }
}