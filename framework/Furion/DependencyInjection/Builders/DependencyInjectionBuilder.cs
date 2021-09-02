// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Furion.DependencyInjection;

/// <summary>
/// 依赖注入构建器
/// </summary>
public class DependencyInjectionBuilder : IDependencyInjectionBuilder
{
    /// <summary>
    /// 服务注册集合对象
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// 命名服务描述器集合
    /// </summary>
    private readonly IDictionary<string, Type> _namedServiceDescriptors = new Dictionary<string, Type>();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public DependencyInjectionBuilder(IServiceCollection services, IConfiguration configuration)
    {
        _services = services;
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IDependencyInjectionBuilder AddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        var implementationType = typeof(TImplementation);

        _services.Add(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));
        _services.Add(ServiceDescriptor.Describe(typeof(TService), implementationType, lifetime));

        _namedServiceDescriptors.Add(serviceName, implementationType);

        return this;
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IDependencyInjectionBuilder TryAddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
          where TService : class
        where TImplementation : class, TService
    {
        var implementationType = typeof(TImplementation);

        _services.TryAdd(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));
        _services.TryAdd(ServiceDescriptor.Describe(typeof(TService), implementationType, lifetime));

        _namedServiceDescriptors.TryAdd(serviceName, implementationType);

        return this;
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IDependencyInjectionBuilder AddNamedService<TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TImplementation : class
    {
        var implementationType = typeof(TImplementation);

        _services.Add(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));

        _namedServiceDescriptors.Add(serviceName, implementationType);

        return this;
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IDependencyInjectionBuilder TryAddNamedService<TImplementation>(string serviceName, ServiceLifetime lifetime)
          where TImplementation : class
    {
        var implementationType = typeof(TImplementation);

        _services.TryAdd(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));

        _namedServiceDescriptors.TryAdd(serviceName, implementationType);

        return this;
    }

    /// <summary>
    /// 构建服务
    /// </summary>
    public void Build()
    {
        // 注册命名服务提供器
        _services.AddTransient<INamedServiceProvider>(sp => new NamedServiceProvider(sp.Resolve(), _namedServiceDescriptors));
    }
}