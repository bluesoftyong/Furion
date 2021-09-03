// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.ObjectExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using System.Reflection;

namespace Furion.DependencyInjection;

/// <summary>
/// 依赖注入构建器
/// </summary>
public sealed class ServiceBuilder : IServiceBuilder
{
    /// <summary>
    /// 服务注册集合对象
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// 命名服务集合
    /// </summary>
    private readonly IDictionary<string, Type> _namedServiceCollection;

    /// <summary>
    /// 依赖注入扫描程序集集合
    /// </summary>
    private readonly IDictionary<Assembly, Assembly> _additionAssemblies;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="services"></param>
    /// <param name="contextProperties"></param>
    public ServiceBuilder(IServiceCollection services
        , IDictionary<object, object> contextProperties)
    {
        _services = services;
        _additionAssemblies = (contextProperties["AdditionAssemblies"] as IDictionary<Assembly, Assembly>)!;
        _namedServiceCollection = (contextProperties["NamedServiceCollection"] as IDictionary<string, Type>)!;
    }

    /// <summary>
    /// 添加依赖注入扫描程序集
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public IServiceBuilder AddAssemblies(params Assembly[] assemblies)
    {
        if (assemblies.IsEmpty()) throw new ArgumentException(nameof(assemblies));

        Array.ForEach(assemblies, ass => _additionAssemblies.Add(ass, ass));

        return this;
    }

    /// <summary>
    /// 添加依赖注入扫描程序集
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public IServiceBuilder TryAddAssemblies(params Assembly[] assemblies)
    {
        if (assemblies.IsEmpty()) throw new ArgumentException(nameof(assemblies));

        Array.ForEach(assemblies, ass => _additionAssemblies.TryAdd(ass, ass));

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
    public IServiceBuilder AddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        var implementationType = typeof(TImplementation);

        _services.Add(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));
        _services.Add(ServiceDescriptor.Describe(typeof(TService), implementationType, lifetime));

        _namedServiceCollection.Add(serviceName, implementationType);

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
    public IServiceBuilder TryAddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
          where TService : class
        where TImplementation : class, TService
    {
        var implementationType = typeof(TImplementation);

        _services.TryAdd(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));
        _services.TryAdd(ServiceDescriptor.Describe(typeof(TService), implementationType, lifetime));

        _namedServiceCollection.TryAdd(serviceName, implementationType);

        return this;
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IServiceBuilder AddNamedService<TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TImplementation : class
    {
        var implementationType = typeof(TImplementation);

        _services.Add(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));

        _namedServiceCollection.Add(serviceName, implementationType);

        return this;
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IServiceBuilder TryAddNamedService<TImplementation>(string serviceName, ServiceLifetime lifetime)
          where TImplementation : class
    {
        var implementationType = typeof(TImplementation);

        _services.TryAdd(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));

        _namedServiceCollection.TryAdd(serviceName, implementationType);

        return this;
    }

    /// <summary>
    /// 构建服务
    /// </summary>
    public void Build()
    {
        // 注册命名服务提供器
        _services.AddTransient<INamedServiceProvider>(provider => new NamedServiceProvider(provider.CreateProxy(), _namedServiceCollection));

        Trace.WriteLine(string.Join(";\n", _namedServiceCollection.Select(c => $"{c.Key} = {c.Value}")));
    }
}