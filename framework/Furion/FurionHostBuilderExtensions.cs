// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 配置 Furion 框架初始化
/// </summary>
public static class FurionHostBuilderExtensions
{
    /// <summary>
    /// 初始化框架服务
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <param name="configure">服务提供器配置</param>
    /// <returns></returns>
    public static IHostBuilder UseFurion(this IHostBuilder hostBuilder, Action<HostBuilderContext, AppServiceProviderOptions>? configure = default)
    {
        // 添加框架初始配置
        hostBuilder.AddAppConfiguration();

        // 配置初始服务
        hostBuilder.ConfigureServices((hostBuilderContext, services) =>
        {
            // 注册 HttpContext 访问器
            services.AddHttpContextAccessor();

            // 注册 App 全局应用对象服务
            services.AddApp(hostBuilderContext.Configuration);

            // 注册框架服务提供器
            services.TryAddTransient(sp => sp.Resolve());
        });

        // 配置框架服务提供器工厂
        hostBuilder.UseAppServiceProviderFactory(configure);

        return hostBuilder;
    }

    /// <summary>
    /// 初始化框架服务（含常用服务注册）
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <returns></returns>
    public static IHostBuilder UseFurionDefaults(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseFurion();

        // 配置默认服务
        hostBuilder.ConfigureServices((hostBuilderContext, services) =>
        {
            // 依赖注入
            services.AddDependencyInjectionServices(hostBuilderContext.Configuration);
        });

        return hostBuilder;
    }
}