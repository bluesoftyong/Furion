// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 依赖注入服务拓展
/// </summary>
public static class DependencyInjectionServiceCollectionExtensions
{
    /// <summary>
    /// 依赖注入服务拓展
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="builderConfigure"></param>
    /// <returns></returns>
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration, Action<IDependencyInjectionBuilder>? builderConfigure = default)
    {
        if (builderConfigure != default)
        {
            var builder = new DependencyInjectionBuilder(services, configuration);
            builderConfigure(builder);
            builder.Build();
        }

        return services;
    }
}