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
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App 模块服务拓展
/// </summary>
public static class AppServiceCollectionExtensions
{
    /// <summary>
    /// 添加 App 模块注册
    /// </summary>
    /// <param name="services">服务集合对象</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>服务集合实例</returns>
    public static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册 App 模块选项配置
        services.AddOptions<AppSettingsOptions>().ConfigureBuilder(configuration);

        // 注册 App 模块 IApp 单例服务
        services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IApp), typeof(App)));

        return services;
    }
}