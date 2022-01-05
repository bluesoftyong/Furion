// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace System;

/// <summary>
/// 命名服务提供器
/// </summary>
public interface INamedServiceProvider
{
    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    object? GetService(string serviceName);

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    object GetRequiredService(string serviceName);

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    T? GetService<T>(string serviceName)
        where T : class;

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    T GetRequiredService<T>(string serviceName)
        where T : class;
}