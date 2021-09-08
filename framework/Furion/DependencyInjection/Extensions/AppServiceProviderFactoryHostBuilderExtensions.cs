// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion;
using Furion.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 框架内服务提供器工厂主机拓展类
/// </summary>
internal static class AppServiceProviderFactoryHostBuilderExtensions
{
    /// <summary>
    /// 使用框架提供的服务提供器工厂
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    internal static IHostBuilder UseAppServiceProviderFactory(this IHostBuilder hostBuilder, Action<HostBuilderContext, ServiceProviderOptions>? configureDelegate = default)
    {
        // 配置服务提供器，创建一个全局服务构建器并添加到上下文共享数据集合中
        hostBuilder.ConfigureContainer<IServiceCollection>((context, services) =>
        {
            var serviceBuilder = new ServiceBuilder(context);
            context.Properties.Add(FurionConsts.HOST_PROPERTIES_SERVICE_BUILDER, serviceBuilder);

            serviceBuilder.AddAssemblies(Assembly.GetEntryAssembly()!);
            serviceBuilder.Build(services);

            // 实现属性注入
            services.AddPropertiesAutowired();
        });

        // 替换 .NET 默认工厂
        return hostBuilder.UseServiceProviderFactory(context =>
        {
            // 创建默认配置选项
            var serviceProviderOptions = new ServiceProviderOptions();
            var validateOnBuild = (serviceProviderOptions.ValidateScopes = context.HostingEnvironment.IsDevelopment());
            serviceProviderOptions.ValidateOnBuild = validateOnBuild;

            configureDelegate?.Invoke(context, serviceProviderOptions);

            return new AppServiceProviderFactory(serviceProviderOptions);
        });
    }

    /// <summary>
    /// 实现属性注入
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection AddPropertiesAutowired(this IServiceCollection services)
    {
        // 替换 Mvc 控制器激活器
        if (services.Any(u => u.ServiceType == typeof(IControllerActivator)))
        {
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, AppControllerActivator>());
        }

        // 替换所有 IHostedService 服务并实现属性注入
        var hostedServiceDescriptors = services.Where(u => u.ServiceType == typeof(IHostedService) && u.ImplementationType != default).ToList();
        foreach (var serviceDescriptor in hostedServiceDescriptors)
        {
            services.Replace(ServiceDescriptor.Describe(serviceDescriptor.ServiceType, provider =>
            {
                var appServiceProvider = provider.CreateProxy();
                return ActivatorUtilities.CreateInstance(appServiceProvider, serviceDescriptor.ImplementationType!);
            }, serviceDescriptor.Lifetime));
        }

        return services;
    }
}