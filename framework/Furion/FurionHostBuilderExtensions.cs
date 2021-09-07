// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion;
using Furion.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using System.Reflection;

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
    public static IHostBuilder UseFurion(this IHostBuilder hostBuilder, Action<HostBuilderContext, ServiceProviderOptions>? configure = default)
    {
        // 配置框架诊断监听器
        DiagnosticListener.AllListeners.Subscribe(new FurionDiagnosticObserver());

        // 添加框架初始配置
        hostBuilder.AddAppConfiguration();

        // 替换 .NET 内置默认服务提供器工厂
        hostBuilder.UseAppServiceProviderFactory(configure);

        // 配置初始服务
        hostBuilder.ConfigureServices((context, services) =>
        {
            // 注册 App 全局应用对象服务
            services.AddApp(context.Configuration);

            // 注册框架服务提供器
            services.TryAddTransient(provider => provider.CreateProxy());

            // 添加启动程序集到服务构建器中
            context.Properties.Add(FurionConsts.HOST_PROPERTIES_SERVICE_BUILDER,
                new ServiceBuilder(context.Properties).AddAssemblies(Assembly.GetEntryAssembly()!));
        });

        return hostBuilder;
    }

    /// <summary>
    /// 添加框架初始配置
    /// </summary>
    /// <param name="hostBuilder">主机构建器</param>
    /// <returns></returns>
    internal static IHostBuilder AddAppConfiguration(this IHostBuilder hostBuilder)
    {
        hostBuilder.Properties.Add(FurionConsts.HOST_PROPERTIES_NAMED_SERVICE_COLLECTION, new Dictionary<string, Type>());
        hostBuilder.Properties.Add(FurionConsts.HOST_PROPERTIES_ADDITION_ASSEMBLIES, new Dictionary<Assembly, Assembly>());
        hostBuilder.Properties.Add(FurionConsts.HOST_PROPERTIES_SERVICE_DESCRIPTORS, new Dictionary<ServiceDescriptor, ServiceDescriptor>());

        hostBuilder.ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
        {
            hostingContext.Properties.Add(FurionConsts.HOST_PROPERTIES_HOST_BUILDER_CONTEXT, hostingContext);
            configurationBuilder.Configure(hostingContext.Configuration, hostingContext.HostingEnvironment);
        });

        return hostBuilder;
    }
}