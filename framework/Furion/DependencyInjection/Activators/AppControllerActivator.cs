// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.Controllers;

/// <summary>
/// 框架控制器激活器
/// </summary>
internal sealed class AppControllerActivator : IControllerActivator
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

        var autowiredServiceProvider = controllerContext.HttpContext.RequestServices.Autowired();
        var controller = CreateInstance(controllerTypeInfo, autowiredServiceProvider);

        return autowiredServiceProvider.ResolveAutowriedService(controller)!;
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

    /// <summary>
    /// 创建控制器实例
    /// </summary>
    /// <param name="controllerTypeInfo"></param>
    /// <param name="autowiredServiceProvider"></param>
    /// <returns></returns>
    private static object CreateInstance(TypeInfo controllerTypeInfo, IAutowiredServiceProvider autowiredServiceProvider)
    {
        // 获取 .NET 内部 ITypeActivatorCache 接口实例
        var typeActivatorCache = autowiredServiceProvider.GetRequiredService(typeof(IActionSelector).Assembly
                                            .GetType("Microsoft.AspNetCore.Mvc.Infrastructure.ITypeActivatorCache")!);

        // 构建表达式并生成方法委托
        var arg0 = Expression.Parameter(typeof(IServiceProvider), "arg0");
        var arg1 = Expression.Parameter(typeof(Type), "arg1");
        var callMethod = Expression.Call(Expression.Constant(typeActivatorCache), "CreateInstance", new[] { typeof(object) }, arg0, arg1);
        var @delegate = Expression.Lambda<Func<IServiceProvider, Type, object>>(callMethod, arg0, arg1).Compile();

        // 创建控制器
        var controller = @delegate(autowiredServiceProvider, controllerTypeInfo.AsType());
        return controller;
    }
}