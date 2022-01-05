// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Furion.DependencyInjection;

/// <summary>
/// 默认拦截器
/// </summary>
public class DefaultInterceptor : DispatchProxy
{
    public object? Target { get; internal set; }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        var isAsyncMethod = targetMethod!.GetCustomAttribute<AsyncMethodBuilderAttribute>() != null;
        if (!isAsyncMethod)
        {
            return Intercept(targetMethod!, args!);
        }

        var returnType = targetMethod!.ReturnType;
        if (!returnType.IsGenericType)
        {
            var task = InterceptAsync(targetMethod, args!);
        }
        else
        {
            var interceptAsyncMethod = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                         .First(m => m.Name == nameof(InterceptAsync) && m.IsGenericMethod)
                                                         .MakeGenericMethod(returnType.GetGenericArguments());
            var task = interceptAsyncMethod.Invoke(this, new object[] { targetMethod, args! });
        }

        return default;
    }

    public virtual object? Intercept(MethodInfo targetMethod, object[] args)
    {
        return targetMethod.Invoke(Target, args);
    }

    public virtual Task InterceptAsync(MethodInfo targetMethod, object[] args)
    {
        return (targetMethod.Invoke(Target, args) as Task)!;
    }

    public virtual Task<TResult> InterceptAsync<TResult>(MethodInfo targetMethod, object[] args)
    {
        return (targetMethod.Invoke(Target, args) as Task<TResult>)!;
    }
}