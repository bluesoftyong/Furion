// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Furion.ObjectExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Furion.DependencyInjection;

/// <summary>
/// 依赖注入构建器
/// </summary>
internal sealed class ServiceBuilder : IServiceBuilder
{
    /// <summary>
    /// 主机构建器上下文
    /// </summary>
    private readonly HostBuilderContext _context;

    /// <summary>
    /// 命名服务集合
    /// </summary>
    private readonly ConcurrentDictionary<string, Type> _namedServiceCollection;

    /// <summary>
    /// 服务描述器集合
    /// </summary>
    private readonly ConcurrentDictionary<Type, ServiceDescriptor> _serviceDescriptors;

    /// <summary>
    /// 依赖注入扫描程序集集合
    /// </summary>
    private readonly ConcurrentDictionary<Assembly, Assembly> _additionAssemblies;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context"></param>
    internal ServiceBuilder(HostBuilderContext context)
    {
        _context = context;

        _additionAssemblies = new ConcurrentDictionary<Assembly, Assembly>();
        _namedServiceCollection = new ConcurrentDictionary<string, Type>();
        _serviceDescriptors = new ConcurrentDictionary<Type, ServiceDescriptor>();
    }

    /// <summary>
    /// 添加程序集
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public IServiceBuilder AddAssemblies(params Assembly[] assemblies)
    {
        if (assemblies.IsEmpty())
        {
            throw new ArgumentException(nameof(assemblies));
        }

        Parallel.ForEach(assemblies, ass => _additionAssemblies.TryAdd(ass, ass));

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
        return AddAssemblies(assemblies);
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="implementationType"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IServiceBuilder AddNamedService(string serviceName, Type implementationType, ServiceLifetime lifetime)
    {
        // 如果存在，则覆盖（更新）
        _namedServiceCollection.AddOrUpdate(serviceName, implementationType, (_, _) => implementationType);
        _serviceDescriptors.TryAdd(implementationType, ServiceDescriptor.Describe(implementationType, implementationType, lifetime));

        return this;
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="implementationType"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public IServiceBuilder TryAddNamedService(string serviceName, Type implementationType, ServiceLifetime lifetime)
    {
        if (_namedServiceCollection.TryAdd(serviceName, implementationType))
        {
            _serviceDescriptors.TryAdd(implementationType, ServiceDescriptor.Describe(implementationType, implementationType, lifetime));
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
        return AddNamedService(serviceName, typeof(TImplementation), lifetime);
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
        return TryAddNamedService(serviceName, typeof(TImplementation), lifetime);
    }

    /// <summary>
    /// 服务注册构建
    /// </summary>
    /// <param name="services"></param>
    internal void Build(IServiceCollection services)
    {
        // 注册命名服务提供器
        services.AddTransient<INamedServiceProvider>(provider => ActivatorUtilities.CreateInstance<NamedServiceProvider>(provider, _namedServiceCollection));

        // 通过主机构建器服务依赖接口批量注册
        ParallelLoopResult _1() => BatchRegisterHostBuilderServices(services);

        // 通过依赖接口批量注册
        ParallelLoopResult _2() => BatchRegisterServiceTypes(services);

        // 通过依赖工厂类型批量注册
        ParallelLoopResult _3() => BatchRegisterFactoryServiceTypes(services);

        // 通过服务描述器批量注册
        ParallelLoopResult _4() => BatchRegisterServiceDescriptors(services);

        // 等待任务完成释放服务构建器
        _1().ContinueWith(new Func<ParallelLoopResult>[] { _2, _3, _4 }, () =>
        {
            _additionAssemblies.Clear();
            _serviceDescriptors.Clear();
            _context.Properties.Remove(FurionConsts.HOST_PROPERTIES_SERVICE_BUILDER);
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
    /// 通过依赖类型批量注册
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private ParallelLoopResult BatchRegisterServiceTypes(IServiceCollection services)
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

            // 获取所有服务类型，排除框架本身程序集接口及 IDisposable/ IAsyncDisposable 接口
            var serviceTypes = interfaces.Where(type => type.Assembly != dependencyType.Assembly && !dependencyType.IsAssignableFrom(type) && type != typeof(IDisposable) && type != typeof(IAsyncDisposable))
                                                        .Select(type => FixedGenericType(type));

            // 如果没有扫描到接口，则注册类型本身
            if (!serviceTypes.Any())
            {
                _serviceDescriptors.TryAdd(implementationType, ServiceDescriptor.Describe(implementationType, implementationType, lifetime));
            }

            foreach (var serviceType in serviceTypes)
            {
                services.Add(ServiceDescriptor.Describe(serviceType, implementationType, lifetime));
            }

            // 注册命名服务
            RegisterNamedService(implementationType, lifetime);
        });
    }

    /// <summary>
    /// 通过依赖工厂类型批量注册
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private ParallelLoopResult BatchRegisterFactoryServiceTypes(IServiceCollection services)
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

            var realityServiceType = serviceType == typeof(object) ? implementationType : serviceType;
            services.Add(ServiceDescriptor.Describe(realityServiceType, provider =>
            {
                var appServiceProvider = provider.CreateProxy();
                var instance = implementationFactory(appServiceProvider);
                return appServiceProvider.ResolveAutowriedService(instance)!;
            }, lifetime));

            // 注册命名服务
            RegisterNamedService(implementationType, lifetime);
        });
    }

    /// <summary>
    /// 通过主机构建器服务依赖接口批量注册
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private ParallelLoopResult BatchRegisterHostBuilderServices(IServiceCollection services)
    {
        var hostBuilderServiceDependencyType = typeof(IHostBuilderService);

        // 扫描项目所有程序集公开类型
        var hostBuilderServiceTypes = GetProjectAssemblies().SelectMany(
                                                    ass => ass.ExportedTypes.Where(type => !type.IsAbstract && !type.IsInterface && type.IsClass && hostBuilderServiceDependencyType.IsAssignableFrom(type)));

        return Parallel.ForEach(hostBuilderServiceTypes, hostBuilderServiceType =>
        {
            // 获取配置服务委托
            var configureDelegate = hostBuilderServiceType.GetTypeInfo().DeclaredMethods
                                                           .Single(m => (m.Name == "Configure" || m.Name.EndsWith($".Configure")) && m.GetParameters()[0].ParameterType == typeof(IServiceCollection))
                                                           .CreateDelegate<Action<IServiceCollection, HostBuilderContext>>(Convert.ChangeType(default, hostBuilderServiceType));

            configureDelegate(services, _context);
        });
    }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <param name="implementationType"></param>
    /// <param name="lifetime"></param>
    private void RegisterNamedService(Type implementationType, ServiceLifetime lifetime)
    {
        if (!typeof(INamedService).IsAssignableFrom(implementationType))
        {
            return;
        }

        // 获取类型服务名称委托
        var serviceNameDelegate = implementationType.GetTypeInfo().DeclaredMethods
                                                       .Single(m => (m.Name == "ServiceName" || m.Name.EndsWith($".ServiceName")))
                                                       .CreateDelegate<Func<string>>(Convert.ChangeType(default, implementationType));

        AddNamedService(serviceNameDelegate(), implementationType, lifetime);
    }

    /// <summary>
    /// 根据依赖接口类型解析 ServiceLifetime 对象
    /// </summary>
    /// <param name="dependencyLifetimeType"></param>
    /// <returns></returns>
    private static ServiceLifetime ConvertToServiceLifetime(Type dependencyLifetimeType)
    {
        if (dependencyLifetimeType == typeof(ITransientService))
        {
            return ServiceLifetime.Transient;
        }
        else if (dependencyLifetimeType == typeof(IScopedService))
        {
            return ServiceLifetime.Scoped;
        }
        else if (dependencyLifetimeType == typeof(ISingletonService))
        {
            return ServiceLifetime.Singleton;
        }
        else
        {
            throw new InvalidCastException("Invalid service registration lifetime.");
        }
    }

    /// <summary>
    /// 修正泛型类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static Type FixedGenericType(Type type)
    {
        if (!type.IsGenericType)
        {
            return type;
        }

        return type.Assembly.GetType($"{type.Namespace}.{type.Name}")!;
    }

    /// <summary>
    /// 获取项目所有程序集
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Assembly> GetProjectAssemblies()
    {
        var excludeAssemblyNames = new[] { "Microsoft.", "System.", "netstandard", "mscorlib" };

        var projectAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(ass => !ass.IsDynamic && ass.GetName().Name != nameof(Furion) && !excludeAssemblyNames.Any(u => ass.GetName().Name!.StartsWith(u, StringComparison.OrdinalIgnoreCase)));

        return projectAssemblies;
    }
}