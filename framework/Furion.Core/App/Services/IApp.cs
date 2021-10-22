// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Furion.App;

/// <summary>
/// App 模块全局单例服务接口
/// </summary>
public interface IApp
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 配置对象
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// 主机环境对象
    /// </summary>
    IHostEnvironment Environment { get; }

    /// <summary>
    /// 主机对象
    /// </summary>
    /// <remarks>可通过.Services 获取根服务，常用于多线程操作</remarks>
    IHost Host { get; }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>object实例或null</returns>
    object? GetService(Type serviceType);

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>object实例或异常</returns>
    object GetRequiredService(Type serviceType);

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService">服务类型，约束为引用类型</typeparam>
    /// <returns>服务实例或null</returns>
    TService? GetService<TService>()
       where TService : class;

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService">服务类型，约束为引用类型</typeparam>
    /// <returns>服务实例或异常</returns>
    TService GetRequiredService<TService>()
       where TService : class;
}