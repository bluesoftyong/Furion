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
using Microsoft.Extensions.Logging;
using System;

namespace Furion;

/// <summary>
/// App 全局应用对象实现类
/// </summary>
internal sealed partial class App : IApp
{
    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger<App> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="configuration"></param>
    /// <param name="hostEnvironment"></param>
    /// <param name="host"></param>
    public App(ILogger<App> logger
        , IServiceProvider serviceProvider
        , IConfiguration configuration
        , IHostEnvironment hostEnvironment
        , IHost host)
    {
        _logger = logger;
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
    /// 主机环境
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
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或Null </returns>
    public TService? GetService<TService>()
        where TService : class
    {
        return ServiceProvider.GetService<TService>();
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或异常 </returns>
    public TService? GetRequiredService<TService>()
        where TService : class
    {
        return ServiceProvider.GetRequiredService<TService>();
    }
}