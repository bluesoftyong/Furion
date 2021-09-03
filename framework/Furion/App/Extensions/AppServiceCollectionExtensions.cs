// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App 全局应用服务拓展类
/// </summary>
internal static class AppServiceCollectionExtensions
{
    /// <summary>
    /// 添加 App 全局应用服务
    /// </summary>
    /// <param name="services">服务注册集合</param>
    /// <param name="configuration">配置对象或配置节点对象</param>
    /// <param name="configureOptions">AppSettings 后置配置</param>
    /// <returns></returns>
    internal static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册 App 全局应用配置
        services.AddAppOptions<AppSettingsOptions>(configuration);

        // 注册全局 IApp 对象
        services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IApp), typeof(App)));

        return services;
    }
}