// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.ObjectExtensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace System;

/// <summary>
/// 框架内服务提供器
/// </summary>
public sealed class AppServiceProvider : IAppServiceProvider
{
    /// <summary>
    /// .NET 内置服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider"></param>
    public AppServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public object? GetService(Type serviceType)
    {
        var instance = _serviceProvider.GetService(serviceType);
        if (instance == default)
        {
            return default;
        }

        // 这里需要排除微软程序集
        //var proxyInstance = typeof(DispatchProxy).GetMethod("Create", BindingFlags.Public | BindingFlags.Static)!.MakeGenericMethod(serviceType, typeof(DefaultInterceptor)).Invoke(null, Array.Empty<object>()) as DefaultInterceptor;
        //proxyInstance!.Target = instance;

        return ResolveAutowriedService(instance);
    }

    /// <summary>
    /// 属性注入服务
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public object? ResolveAutowriedService(object? instance)
    {
        if (instance == null)
        {
            return default;
        }

        var instanceType = instance as Type ?? instance.GetType();

        // 扫描所有公开和非公开的实例属性
        var serviceProperties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                          .Where(p => (p.PropertyType.IsClass || p.PropertyType.IsInterface) && p.IsDefined(typeof(AutowiredServicesAttribute), false));

        if (serviceProperties.IsEmpty())
        {
            return instance;
        }

        Parallel.ForEach(serviceProperties, p =>
        {
            var autowiredServicesAttributes = p.GetCustomAttribute<AutowiredServicesAttribute>(false);
            if (autowiredServicesAttributes?.Required ?? true)
            {
                p.SetPropertyValue(instance, _serviceProvider.GetRequiredService(p.PropertyType));
            }
            else
            {
                p.SetPropertyValue(instance, GetService(p.PropertyType));
            }
        });

        return instance;
    }
}