// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
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
public sealed class NamedServiceProvider : INamedServiceProvider
{
    /// <summary>
    /// 框架内服务提供器
    /// </summary>
    private readonly IAppServiceProvider _appServiceProvider;

    /// <summary>
    /// 命名服务集合
    /// </summary>
    private readonly IDictionary<string, Type> _namedServiceCollection;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="appServiceProvider"></param>
    /// <param name="namedServiceCollection"></param>
    public NamedServiceProvider(IAppServiceProvider appServiceProvider
        , IDictionary<string, Type> namedServiceCollection)
    {
        _appServiceProvider = appServiceProvider;
        _namedServiceCollection = namedServiceCollection;
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    public object? GetService(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentNullException(nameof(serviceName));
        }

        var isRegistered = _namedServiceCollection.TryGetValue(serviceName, out var implementactionType);
        return isRegistered ? _appServiceProvider.GetService(implementactionType!) : default;
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    public object GetRequiredService(string serviceName)
    {
        return GetService(serviceName) ?? throw new InvalidOperationException($"Named service `{serviceName}` is not registered in container.");
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public T? GetService<T>(string serviceName)
         where T : class
    {
        return GetService(serviceName) as T;
    }

    /// <summary>
    /// 解析服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public T GetRequiredService<T>(string serviceName)
         where T : class
    {
        return (GetRequiredService(serviceName) as T)!;
    }
}