// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace System;

/// <summary>
/// IServiceProvider 拓展类
/// </summary>
public static class IServiceProviderExtensions
{
    /// <summary>
    /// 创建服务提供器代理
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IAppServiceProvider CreateProxy(this IServiceProvider serviceProvider)
    {
        if (serviceProvider is AppServiceProvider)
        {
            return (serviceProvider as IAppServiceProvider)!;
        }
        else
        {
            return new AppServiceProvider(serviceProvider);
        }
    }
}