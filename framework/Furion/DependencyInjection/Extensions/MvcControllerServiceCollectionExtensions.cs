// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 控制器服务拓展类
/// </summary>
public static class MvcControllerServiceCollectionExtensions
{
    /// <summary>
    /// 支持属性注入的控制器服务
    /// </summary>
    /// <param name="mvcBuilder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IMvcBuilder AddAutowired(this IMvcBuilder mvcBuilder)
    {
        var services = mvcBuilder.Services;

        // 替换控制器激活器
        services.Replace(ServiceDescriptor.Transient<IControllerActivator, AppControllerActivator>());

        return mvcBuilder;
    }
}