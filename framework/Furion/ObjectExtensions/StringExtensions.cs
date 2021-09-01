namespace Furion.ObjectExtensions;

/// <summary>
/// 字符串拓展
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// 移除字符串指定后缀
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="suffix">后缀字符串</param>
    /// <param name="comparisonType">字符串比较方式</param>
    /// <returns></returns>
    internal static string SubSuffix(this string str, string suffix, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (!str.EndsWith(suffix, comparisonType)) return str;

        return str[0..^suffix.Length];
    }

    /// <summary>
    /// 首字母大写
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns></returns>
    internal static string ToTitleCase(this string str)
    {
        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}