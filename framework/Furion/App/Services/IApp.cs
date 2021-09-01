// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Furion;

/// <summary>
/// App 全局应用对象接口规范
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
    /// 主机环境
    /// </summary>
    IHostEnvironment Environment { get; }

    /// <summary>
    /// 主机对象
    /// </summary>
    /// <remarks>可通过.Services 获取根服务，常用于多线程操作</remarks>
    IHost Host { get; }

    /// <summary>
    /// App 全局配置
    /// </summary>
    AppSettingsOptions AppSettings { get; }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或Null </returns>
    TService? GetService<TService>()
       where TService : class;

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns> 服务实现类或异常 </returns>
    TService? GetRequiredService<TService>()
       where TService : class;
}