using System.Reflection;

namespace Furion.ObjectExtensions;

internal static class TypeExtensions
{
    /// <summary>
    /// 获取类型自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="classType">类类型</param>
    /// <param name="inherit">是否继承查找</param>
    /// <returns>特性对象</returns>
    internal static TAttribute? GetTypeAttribute<TAttribute>(this Type classType, bool inherit = false)
        where TAttribute : Attribute
    {
        return classType.IsDefined(typeof(TAttribute), inherit)
            ? classType.GetCustomAttribute<TAttribute>(inherit)
            : default;
    }
}