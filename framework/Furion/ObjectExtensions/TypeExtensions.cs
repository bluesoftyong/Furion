// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Reflection;

namespace Furion.ObjectExtensions;

/// <summary>
/// Type 类型拓展
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// 获取类型自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="classType">类类型</param>
    /// <param name="inherit">是否继承查找</param>
    /// <returns>特性对象</returns>
    internal static TAttribute? GetTypeAttribute<TAttribute>(this Type? classType, bool inherit = false)
        where TAttribute : Attribute
    {
        if (classType == null) throw new ArgumentNullException(nameof(classType));

        return classType.IsDefined(typeof(TAttribute), inherit)
            ? classType.GetCustomAttribute<TAttribute>(inherit)
            : default;
    }
}