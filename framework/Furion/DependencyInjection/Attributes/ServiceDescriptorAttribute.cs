// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 服务描述器特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ServiceDescriptorAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ServiceDescriptorAttribute()
    {
    }

    /// <summary>
    /// 命名服务配置名称
    /// </summary>
    /// <remarks>优先级低于 <see cref="INamedService.ServiceName"/> </remarks>
    public string? ServiceName { get; set; }

    /// <summary>
    /// 注册类型
    /// </summary>
    public RegisterTypes RegisterType { get; set; } = RegisterTypes.Add;

    /// <summary>
    /// 配置注册接口类型
    /// </summary>
    /// <remarks>如果指定则优先选用，否则扫描所有接口并全部注册</remarks>
    public Type[]? ServiceTypes { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>序号越小，越先注册</remarks>
    public int Order { get; set; } = 0;
}