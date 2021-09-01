// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Furion.Options.Extensions;

/// <summary>
/// OptionsBuilder 对象拓展类
/// </summary>
internal static class OptionsBuilderExtensions
{
    /// <summary>
    /// 反射调用 PostConfigure 配置方法
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="optionsBuilder">选项类型</param>
    /// <param name="services">服务注册集合</param>
    /// <returns>OptionsBuilder</returns>
    internal static OptionsBuilder<TOptions>? PostConfigure<TOptions>(this OptionsBuilder<TOptions> optionsBuilder, IServiceCollection services)
        where TOptions : class, IAppOptionsDependency
    {
        var optionsType = typeof(TOptions);

        // 扫描后期配置方法
        var postConfigureMethods = optionsType.GetTypeInfo().DeclaredMethods
                                                            .Where(m => (m.Name == nameof(IAppOptions<TOptions>.PostConfigure) || m.Name.EndsWith($".{nameof(IAppOptions<TOptions>.PostConfigure)}"))
                                                                && m.GetParameters()[0].ParameterType == optionsType);

        // 限制选项多次实现 IAppOptionsDependency 接口
        if (postConfigureMethods.Count() > 1)
            throw new InvalidOperationException($"Please ensure that the option class `{optionsType.Name}` has and uniquely implements the `{nameof(IAppOptionsDependency)}` interface.");

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