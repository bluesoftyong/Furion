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
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Extensions.Options;

/// <summary>
/// OptionsBuilder 拓展类
/// </summary>
public static class OptionsBuilderExtensions
{
    /// <summary>
    /// 添加选项构建器
    /// </summary>
    /// <typeparam name="TOptions">选项类型</typeparam>
    /// <param name="optionsBuilder">选项构建器实例</param>
    /// <param name="builderType">选项构建器类型</param>
    /// <returns>OptionsBuilder{TOptions}</returns>
    public static OptionsBuilder<TOptions> AddBuilder<TOptions>(this OptionsBuilder<TOptions> optionsBuilder, Type builderType)
        where TOptions : class
    {
        // 获取构建器类型接口
        var builderInterface = builderType.GetInterfaces().FirstOrDefault(u => typeof(IOptionsBuilder).IsAssignableFrom(u));
        if (builderInterface == default)
        {
            return optionsBuilder;
        }

        // 获取泛型定义参数
        var genericArguments = builderInterface.GetGenericArguments();

        // 创建 Action 和 Func 参数类型
        var actionType = TypeHelpers.CreateAction(genericArguments);
        var funcType = TypeHelpers.CreateFunc(typeof(bool), genericArguments);

        // 循环调用方法进行初始化配置
        Array.ForEach(new[] { "Configure", "PostConfigure", "Validate" }, methodName =>
        {
            var argumentType = !methodName.Equals("Validate") ? actionType : funcType;
            CallMethod(optionsBuilder, builderInterface, methodName, argumentType, genericArguments);
        });

        return optionsBuilder;
    }

    /// <summary>
    /// 创建表达式动态调用 OptionsBuilder{TOptions} 对应方法
    /// </summary>
    /// <param name="optionsBuilder">选项构建器实例</param>
    /// <param name="builderInterface">选项构建器接口</param>
    /// <param name="methodName">方法名称</param>
    /// <param name="argumentType">参数类型</param>
    /// <param name="genericArguments">泛型参数</param>
    private static void CallMethod(object optionsBuilder, Type builderInterface, string methodName, Type argumentType, params Type[]? genericArguments)
    {
        // 创建方法调用参数
        var argument = builderInterface.GetTypeInfo().DeclaredMethods
                .First(u => u.Name == methodName || u.Name.EndsWith("." + methodName))
                .CreateDelegate(argumentType, default);

        // 创建委托参数表达式
        var argumentExpression = Expression.Parameter(argumentType, "configureDelegate");

        // 创建调用方法表达式
        var callExpression = Expression.Call(Expression.Constant(optionsBuilder), methodName, genericArguments.IsEmpty() ? default : genericArguments!.Skip(1).ToArray(), new[] { argumentExpression });

        // 创建调用委托
        var @delegate = Expression.Lambda(callExpression, argumentExpression).Compile();

        // 动态调用方法
        @delegate.DynamicInvoke(argument);
    }
}