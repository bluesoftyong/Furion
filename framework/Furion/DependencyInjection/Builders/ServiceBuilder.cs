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
using System.Reflection;

namespace Furion.DependencyInjection;

/// <summary>
/// 依赖注入构建器
/// </summary>
internal sealed class ServiceBuilder : IServiceBuilder
{
    /// <summary>
    /// 上下文共享数据
    /// </summary>
    private readonly IDictionary<object, object> _contextProperties;

    /// <summary>
    /// 命名服务集合
    /// </summary>
    private readonly IDictionary<string, Type> _namedServiceCollection;

    /// <summary>
    /// 服务描述器集合
    /// </summary>
    private readonly IDictionary<ServiceDescriptor, ServiceDescriptor> _serviceDescriptors;

    /// <summary>
    /// 依赖注入扫描程序集集合
    /// </summary>
    private readonly IDictionary<Assembly, Assembly> _additionAssemblies;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="services"></param>
    /// <param name="contextProperties"></param>
    internal ServiceBuilder(IDictionary<object, object> contextProperties)
    {
        _contextProperties = contextProperties;
        _additionAssemblies = (contextProperties[FurionConsts.HOST_PROPERTIES_ADDITION_ASSEMBLIES] as IDictionary<Assembly, Assembly>)!;
        _namedServiceCollection = (contextProperties[FurionConsts.HOST_PROPERTIES_NAMED_SERVICE_COLLECTION] as IDictionary<string, Type>)!;
        _serviceDescriptors = (contextProperties[FurionConsts.HOST_PROPERTIES_SERVICE_DESCRIPTORS] as IDictionary<ServiceDescriptor, ServiceDescriptor>)!;
    }

    /// <summary>
    /// 添加依赖注入扫描程序集
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public IServiceBuilder AddAssemblies(params Assembly[] assemblies)
    {
        if (assemblies.IsEmpty()) throw new ArgumentException(nameof(assemblies));

        Parallel.ForEach(assemblies, ass => _additionAssemblies.Add(ass, ass));

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

        Parallel.ForEach(assemblies, ass => _additionAssemblies.TryAdd(ass, ass));

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

        _namedServiceCollection.Add(serviceName, implementationType);

        var implementationServiceDescriptor = ServiceDescriptor.Describe(implementationType, implementationType, lifetime);
        var typeServiceDescriptor = ServiceDescriptor.Describe(typeof(TService), implementationType, lifetime);
        _serviceDescriptors.Add(implementationServiceDescriptor, implementationServiceDescriptor);
        _serviceDescriptors.Add(typeServiceDescriptor, typeServiceDescriptor);

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

        if (_namedServiceCollection.TryAdd(serviceName, implementationType))
        {
            var implementationServiceDescriptor = ServiceDescriptor.Describe(implementationType, implementationType, lifetime);
            var typeServiceDescriptor = ServiceDescriptor.Describe(typeof(TService), implementationType, lifetime);

            _serviceDescriptors.Add(implementationServiceDescriptor, implementationServiceDescriptor);
            _serviceDescriptors.Add(typeServiceDescriptor, typeServiceDescriptor);
        }

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

        _namedServiceCollection.Add(serviceName, implementationType);

        var implementationServiceDescriptor = ServiceDescriptor.Describe(implementationType, implementationType, lifetime);
        _serviceDescriptors.Add(implementationServiceDescriptor, implementationServiceDescriptor);

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

        if (_namedServiceCollection.TryAdd(serviceName, implementationType))
        {
            var implementationServiceDescriptor = ServiceDescriptor.Describe(implementationType, implementationType, lifetime);
            _serviceDescriptors.TryAdd(implementationServiceDescriptor, implementationServiceDescriptor);
        }

        return this;
    }

    /// <summary>
    /// 构建依赖注入程序集注册
    /// </summary>
    /// <param name="services"></param>
    internal void Build(IServiceCollection services)
    {
        var result1 = Parallel.ForEach(_serviceDescriptors.Values, serviceDescriptor => services.Add(serviceDescriptor));

        var dependencyType = typeof(IDependency);
        var serviceTypes = _additionAssemblies.Values.SelectMany(ass => ass.ExportedTypes.Where(type => !type.IsAbstract && !type.IsInterface && type.IsClass
               && dependencyType.IsAssignableFrom(type)));

        var result2 = Parallel.ForEach(serviceTypes, implementationType =>
           {
               var lifetime = ResolveServiceLifetime(implementationType);
               var interfaces = implementationType.GetInterfaces()
                                                   .Where(intr => !dependencyType.IsAssignableFrom(intr) && intr != dependencyType);


               services.Add(ServiceDescriptor.Describe(implementationType, implementationType, lifetime));
               foreach (var serviceType in interfaces)
               {
                   services.Add(ServiceDescriptor.Describe(serviceType, implementationType, lifetime));
               }
           });

        Release(result1, result2);
    }

    /// <summary>
    /// 解析服务种注册生存周期
    /// </summary>
    /// <param name="implementationType"></param>
    /// <returns></returns>
    private static ServiceLifetime ResolveServiceLifetime(Type implementationType)
    {
        ServiceLifetime lifetime;
        if (typeof(ITransientService).IsAssignableFrom(implementationType)) lifetime = ServiceLifetime.Transient;
        else if (typeof(IScopedService).IsAssignableFrom(implementationType)) lifetime = ServiceLifetime.Scoped;
        else if (typeof(ISingletonService).IsAssignableFrom(implementationType)) lifetime = ServiceLifetime.Scoped;
        else throw new InvalidCastException("Invalid service registration lifetime.");
        return lifetime;
    }

    /// <summary>
    /// 释放上下文对象
    /// </summary>
    /// <param name="isCompleted"></param>
    private void Release(params ParallelLoopResult[] results)
    {
        while (true)
        {
            if (results.All(u => u.IsCompleted))
            {
                _contextProperties.Remove(FurionConsts.HOST_PROPERTIES_SERVICE_DESCRIPTORS);
                _contextProperties.Remove(FurionConsts.HOST_PROPERTIES_ADDITION_ASSEMBLIES);
                _contextProperties.Remove(FurionConsts.HOST_PROPERTIES_SERVICE_BUILDER);
                _contextProperties.Remove(FurionConsts.HOST_PROPERTIES_HOST_BUILDER_CONTEXT);
                break;
            }
        }
    }
}