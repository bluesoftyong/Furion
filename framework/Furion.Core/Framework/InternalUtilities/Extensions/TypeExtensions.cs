// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Reflection;

namespace Furion.Extensions.InternalUtilities;

/// <summary>
/// <see cref="Type"/> 类型拓展
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
        // 空检查
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        // 检查特性并获取特性对象
        return type.IsDefined(typeof(TAttribute), inherit)
            ? type.GetCustomAttribute<TAttribute>(inherit)
            : default;
    }

    /// <summary>
    /// 判断类型是否实现泛型接口
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="genericInterface">泛型接口</param>
    /// <returns><see cref="bool"/> 实例，true 表示实现泛型接口，false 表示未实现泛型接口</returns>
    internal static bool IsGenericAssignableTo(this Type type, Type genericInterface)
    {
        return Array.Exists(type.GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == genericInterface);
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="propertyInfo">属性对象</param>
    /// <param name="target">目标对象</param>
    /// <param name="value">要设置的属性值</param>
    internal static void SetPropertyValue(this PropertyInfo propertyInfo
        , object target
        , object? value)
    {
        // 空检查
        if (target == default)
        {
            throw new ArgumentNullException(nameof(target));
        }

        // 判断是否是只读属性
        if (propertyInfo.SetMethod == null)
        {
            target.GetType().GetField($"<{propertyInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                  ?.SetValue(target, value);
        }
        // 可写属性
        else
        {
            propertyInfo.SetValue(target, value);
        }
    }
}