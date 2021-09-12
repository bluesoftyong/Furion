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
using System.Reflection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 服务构建器主机拓展类
/// </summary>
internal static class ServiceBuilderHostBuilderExtensions
{
    /// <summary>
    /// 配置服务构建器
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    internal static IHostBuilder ConfigureServiceBuilder(this IHostBuilder hostBuilder)
    {
        // 配置服务提供器，创建一个全局服务构建器并添加到上下文共享数据集合中
        hostBuilder.ConfigureContainer<IServiceCollection>((context, services) =>
        {
            // 添加服务构建器及命名服务支持
            services.AddServiceBuilderProvider(context);

            // 添加属性注入支持
            services.AddPropertiesAutowired(context);
        });

        return hostBuilder;
    }

    /// <summary>
    /// 添加服务构建器及命名服务支持
    /// </summary>
    /// <param name="services"></param>
    /// <param name="context"></param>
    internal static void AddServiceBuilderProvider(this IServiceCollection services, HostBuilderContext context)
    {
        var serviceBuilder = new ServiceBuilder(context);
        context.Properties.Add(FurionConsts.HOST_PROPERTIES_SERVICE_BUILDER, serviceBuilder);

        serviceBuilder.AddAssemblies(Assembly.GetEntryAssembly()!);
        serviceBuilder.Build(services);
    }

    /// <summary>
    /// 添加属性注入支持
    /// </summary>
    /// <param name="services"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    internal static IServiceCollection AddPropertiesAutowired(this IServiceCollection services, HostBuilderContext _)
    {
        // 替换 Mvc 控制器激活器
        if (services.Any(u => u.ServiceType == typeof(IControllerActivator)))
        {
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, AutowiredControllerActivator>());
        }

        // 替换 IHostedService 服务注册方式
        var hostedServiceDescriptors = services.Where(u => u.ServiceType == typeof(IHostedService) && u.ImplementationType != default && u.ImplementationType.Name != "GenericWebHostService").ToList();
        foreach (var serviceDescriptor in hostedServiceDescriptors)
        {
            services.Replace(ServiceDescriptor.Describe(serviceDescriptor.ServiceType, provider =>
            {
                var appServiceProvider = provider.Autowired();
                return ActivatorUtilities.CreateInstance(appServiceProvider, serviceDescriptor.ImplementationType!);
            }, serviceDescriptor.Lifetime));
        }

        hostedServiceDescriptors.Clear();

        return services;
    }
}