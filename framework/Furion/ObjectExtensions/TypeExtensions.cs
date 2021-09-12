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
    /// <param name="type">类类型</param>
    /// <param name="inherit">是否继承查找</param>
    /// <returns>特性对象</returns>
    internal static TAttribute? GetTypeAttribute<TAttribute>(this Type? type, bool inherit = false)
        where TAttribute : Attribute
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return type.IsDefined(typeof(TAttribute), inherit)
            ? type.GetCustomAttribute<TAttribute>(inherit)
            : default;
    }

    /// <summary>
    /// 判断类型是否实现泛型接口
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericInterface"></param>
    /// <returns></returns>
    internal static bool IsGenericAssignableTo(this Type type, Type genericInterface)
    {
        return Array.Exists(type.GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == genericInterface);
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="property"></param>
    /// <param name="target"></param>
    /// <param name="value"></param>
    internal static void SetPropertyValue(this PropertyInfo property, object target, object? value)
    {
        if (target == default)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (property.SetMethod == null)
        {
            target.GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                  ?.SetValue(target, value);
        }
        else
        {
            property.SetValue(target, value);
        }
    }
}