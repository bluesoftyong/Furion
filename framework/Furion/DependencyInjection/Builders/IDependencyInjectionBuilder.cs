// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;

namespace Furion.DependencyInjection;

/// <summary>
/// 依赖注入构建器
/// </summary>
public interface IDependencyInjectionBuilder
{
    /// <summary>
    /// 服务注册集合对象
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IDependencyInjectionBuilder AddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService;

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IDependencyInjectionBuilder TryAddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService;

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IDependencyInjectionBuilder AddNamedService<TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TImplementation : class;

    /// <summary>
    /// 注册命名服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    IDependencyInjectionBuilder TryAddNamedService<TImplementation>(string serviceName, ServiceLifetime lifetime)
        where TImplementation : class;

    /// <summary>
    /// 构建
    /// </summary>
    void Build();
}