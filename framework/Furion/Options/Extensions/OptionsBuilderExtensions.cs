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
    /// <returns>OptionsBuilder</returns>
    internal static OptionsBuilder<TOptions>? InvokePostConfigure<TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class, IAppOptionsDependency
    {
        var optionsType = typeof(TOptions);

        // 添加后置配置
        var postConfigureMethods = optionsType.GetTypeInfo().DeclaredMethods
                                                            .Where(m => (m.Name == nameof(IAppOptions<TOptions>.PostConfigure) || m.Name.EndsWith($".{nameof(IAppOptions<TOptions>.PostConfigure)}"))
                                                                && m.GetParameters()[0].ParameterType == optionsType);

        if (postConfigureMethods.Count() > 1)
            throw new InvalidOperationException($"Please ensure that the option class `{optionsType.Name}` has and uniquely implements the `{nameof(IAppOptionsDependency)}` interface.");

        var postConfigureMethod = postConfigureMethods.First();

        //  获取后缀选项参数
        var parameterTypes = postConfigureMethod.GetParameters().Select(p => p.ParameterType).ToArray();

        // 获取相同签名的方法
        var optionsBuilderPostConfigureMethod = typeof(OptionsBuilder<TOptions>).GetTypeInfo().DeclaredMethods
              .First(m => m.Name == nameof(IAppOptions<TOptions>.PostConfigure)
                  && m.IsPublic && m.IsVirtual && ((m.IsGenericMethod && m.GetGenericArguments().Length == parameterTypes.Length - 1)) || m.GetParameters().Length == parameterTypes.Length);

        if (optionsBuilderPostConfigureMethod == default)
            throw new InvalidOperationException($"The `{postConfigureMethod}` method with the same signature as `OptionsBuilder<TOptions>` was not found.");

        // 创建相同签名委托
        var actionParameterType = typeof(Action).Assembly.GetType($"{typeof(Action).FullName}`{postConfigureMethod.GetParameters().Length}")
                                            ?.MakeGenericType(parameterTypes);
        var @delegate = postConfigureMethod.CreateDelegate(actionParameterType!, default(TOptions));

        // 调用 PostConfigure 方法
        if (optionsBuilderPostConfigureMethod.IsGenericMethod)
            return optionsBuilderPostConfigureMethod.MakeGenericMethod(parameterTypes.Skip(1).ToArray()).Invoke(optionsBuilder, new[] { @delegate }) as OptionsBuilder<TOptions>;
        else
            return optionsBuilderPostConfigureMethod.Invoke(optionsBuilder, new[] { @delegate }) as OptionsBuilder<TOptions>;
    }
}