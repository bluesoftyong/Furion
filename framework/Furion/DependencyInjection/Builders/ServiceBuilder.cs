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
    /// 添加程序集
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
    /// 添加程序集
    /// </summary>
    /// <remarks>如果存在则跳过</remarks>
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
    /// <remarks>如果存在则跳过</remarks>
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
            _serviceDescriptors.Add(implementationServiceDescriptor, implementationServiceDescriptor);
        }

        return this;
    }

    /// <summary>
    /// 服务注册构建
    /// </summary>
    /// <param name="services"></param>
    internal void Build(IServiceCollection services)
    {
        // 注册命名服务提供器
        services.AddTransient<INamedServiceProvider>(provider => new NamedServiceProvider(provider.CreateProxy(), (_contextProperties[FurionConsts.HOST_PROPERTIES_NAMED_SERVICE_COLLECTION] as IDictionary<string, Type>)!));

        // 批量注册服务描述器
        var result1 = BatchRegisterServiceDescriptors(services);

        // 通过依赖接口批量注册
        var result2 = BatchRegisterServiceByDependencyType(services);

        // 通过依赖工厂类型批量注册
        var result3 = BatchRegisterServiceByDependencyFactoryType(services);

        // 释放主机上下文对象
        Release(result1, result2, result3);
    }

    /// <summary>
    /// 通过依赖类型批量注册
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private ParallelLoopResult BatchRegisterServiceByDependencyType(IServiceCollection services)
    {
        var dependencyType = typeof(IDependency);

        // 扫描所有待注册服务类型
        var implementationTypes = _additionAssemblies.Values.SelectMany(
                                            ass => ass.ExportedTypes.Where(type => !type.IsAbstract && !type.IsInterface && type.IsClass && dependencyType.IsAssignableFrom(type))
                                                                                    .Select(type => FixedGenericType(type)));

        return Parallel.ForEach(implementationTypes, implementationType =>
        {
            var interfaces = implementationType.GetInterfaces();
            var lifetime = ConvertToServiceLifetime(interfaces.First(type => dependencyType.IsAssignableFrom(type) && type != dependencyType));

            // 获取所有服务类型
            var serviceTypes = interfaces.Where(type => !dependencyType.IsAssignableFrom(type))
                                                        .Select(type => FixedGenericType(type));

            foreach (var serviceType in serviceTypes)
            {
                services.Add(ServiceDescriptor.Describe(serviceType, implementationType, lifetime));
            }
        });
    }

    /// <summary>
    /// 通过依赖工厂类型批量注册
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private ParallelLoopResult BatchRegisterServiceByDependencyFactoryType(IServiceCollection services)
    {
        var factoryDependencyType = typeof(IFactoryService<,>);

        // 扫描所有待注册服务工厂类型
        var implementationTypes = _additionAssemblies.Values.SelectMany(
                                                    ass => ass.ExportedTypes.Where(type => !type.IsAbstract && !type.IsInterface && type.IsClass && type.IsGenericAssignableTo(factoryDependencyType)));

        return Parallel.ForEach(implementationTypes, implementationType =>
        {
            var interfaces = implementationType.GetInterfaces();
            var factoryDependencyTypeGenericArguments = interfaces.First(type => FixedGenericType(type) == factoryDependencyType).GetGenericArguments();
            var serviceType = factoryDependencyTypeGenericArguments[0];
            var lifetime = ConvertToServiceLifetime(factoryDependencyTypeGenericArguments[1]);

            // 获取类型实现工厂方法（委托）
            var implementationFactory = implementationType.GetTypeInfo().DeclaredMethods
                                                           .Single(m => (m.Name == "ImplementationFactory" || m.Name.EndsWith($".ImplementationFactory")) && m.GetParameters()[0].ParameterType == typeof(IServiceProvider))
                                                           .CreateDelegate<Func<IServiceProvider, object>>(Convert.ChangeType(default, implementationType));

            services.Add(ServiceDescriptor.Describe(serviceType == typeof(object) ? implementationType : serviceType
                , provider =>
                {
                    var appServiceProvider = provider.CreateProxy();
                    var instance = implementationFactory(appServiceProvider);
                    return appServiceProvider.ResolveAutowriedService(instance)!;
                }, lifetime));
        });
    }

    /// <summary>
    /// 批量注册服务描述器
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private ParallelLoopResult BatchRegisterServiceDescriptors(IServiceCollection services)
    {
        return Parallel.ForEach(_serviceDescriptors.Values, serviceDescriptor => services.Add(serviceDescriptor));
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

    /// <summary>
    /// 根据依赖接口类型解析 ServiceLifetime 对象
    /// </summary>
    /// <param name="dependencyLifetimeType"></param>
    /// <returns></returns>
    private static ServiceLifetime ConvertToServiceLifetime(Type dependencyLifetimeType)
    {
        if (dependencyLifetimeType == typeof(ITransientService)) return ServiceLifetime.Transient;
        else if (dependencyLifetimeType == typeof(IScopedService)) return ServiceLifetime.Scoped;
        else if (dependencyLifetimeType == typeof(ISingletonService)) return ServiceLifetime.Singleton;
        else throw new InvalidCastException("Invalid service registration lifetime.");
    }

    /// <summary>
    /// 修正泛型类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static Type FixedGenericType(Type type)
    {
        if (!type.IsGenericType) return type;

        return type.Assembly.GetType($"{type.Namespace}.{type.Name}")!;
    }
}