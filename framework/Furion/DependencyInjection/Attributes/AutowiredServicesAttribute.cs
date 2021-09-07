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
/// 属性依赖注入特性
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AutowiredServicesAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public AutowiredServicesAttribute()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="required"></param>
    public AutowiredServicesAttribute(bool required)
    {
        Required = required;
    }

    /// <summary>
    /// 如果服务不存在抛异常
    /// </summary>
    public bool Required { get; set; } = true;
}