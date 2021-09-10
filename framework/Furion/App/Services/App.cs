// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Furion;

/// <summary>
/// App 模块全局单例服务
/// </summary>
internal sealed partial class App : IApp
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="configuration">配置对象</param>
    /// <param name="hostEnvironment">主机环境对象</param>
    /// <param name="host">主机对象</param>
    public App(IServiceProvider serviceProvider
        , IConfiguration configuration
        , IHostEnvironment hostEnvironment
        , IHost host)
    {
        ServiceProvider = serviceProvider;
        Configuration = configuration;
        Environment = hostEnvironment;
        Host = host;
    }

    /// <summary>
    /// 服务提供器
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 配置对象
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// 主机环境对象
    /// </summary>
    public IHostEnvironment Environment { get; }

    /// <summary>
    /// 主机对象
    /// </summary>
    /// <remarks>可通过.Services 获取根服务，常用于多线程操作</remarks>
    public IHost Host { get; }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>object实例或null</returns>
    public object? GetService(Type serviceType)
    {
        return ServiceProvider.GetService(serviceType);
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>object实例或异常</returns>
    public object GetRequiredService(Type serviceType)
    {
        return ServiceProvider.GetRequiredService(serviceType);
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService">服务类型，约束为引用类型</typeparam>
    /// <returns>服务实例或null</returns>
    public TService? GetService<TService>()
         where TService : class
    {
        return ServiceProvider.GetService<TService>();
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService">服务类型，约束为引用类型</typeparam>
    /// <returns>服务实例或异常</returns>
    public TService GetRequiredService<TService>()
        where TService : class
    {
        return ServiceProvider.GetRequiredService<TService>();
    }
}