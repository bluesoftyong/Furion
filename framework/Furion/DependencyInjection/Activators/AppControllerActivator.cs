// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.Controllers;

/// <summary>
/// 框架控制器激活器
/// </summary>
public sealed class AppControllerActivator : IControllerActivator
{
    /// <summary>
    /// 实现控制器创建过程
    /// </summary>
    /// <param name="controllerContext"></param>
    /// <returns></returns>
    public object Create(ControllerContext controllerContext)
    {
        if (controllerContext == null)
        {
            throw new ArgumentNullException(nameof(controllerContext));
        }
        if (controllerContext.ActionDescriptor == null)
        {
            throw new ArgumentException(nameof(ControllerContext.ActionDescriptor));
        }
        var controllerTypeInfo = controllerContext.ActionDescriptor.ControllerTypeInfo;
        if (controllerTypeInfo == null)
        {
            throw new ArgumentException(nameof(ControllerContext.ActionDescriptor.ControllerTypeInfo));
        }

        // 提供当前服务提供器
        var serviceProvider = controllerContext.HttpContext.RequestServices.Resolve();

        // 获取构造函数
        var constructors = controllerTypeInfo.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        if (constructors.Length > 1)
            throw new InvalidOperationException($"Multiple constructors accepting all given argument types have been found in type '{controllerTypeInfo.Namespace}.{controllerTypeInfo.Name}'. There should only be one applicable constructor.");

        var constructor = constructors.FirstOrDefault();

        // 处理无参构造函数
        if (constructor?.GetParameters()?.Length == 0)
            return serviceProvider.GetRequiredService(controllerTypeInfo);
        // 处理嵌套依赖
        else
        {
            var parameters = constructors.FirstOrDefault()!.GetParameters()
                                                                   .Where(p => p.ParameterType.IsClass || p.ParameterType.IsInterface)
                                                                   .Select(p => serviceProvider.GetRequiredService(p.ParameterType))
                                                                   .ToArray();

            return serviceProvider.ResolveAutowriedService(
                Activator.CreateInstance(controllerTypeInfo, parameters))!;
        }
    }

    /// <summary>
    /// 释放控制器对象
    /// </summary>
    /// <param name="controllerContext"></param>
    /// <param name="controller"></param>
    public void Release(ControllerContext controllerContext, object controller)
    {
        if (controllerContext == null)
        {
            throw new ArgumentNullException(nameof(controllerContext));
        }
        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        (controller as IDisposable)?.Dispose();
    }
}
