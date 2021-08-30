namespace Furion.ObjectExtensions;

/// <summary>
/// 字符串拓展
/// </summary>
internal static class StringExtensions
{
    internal static string SubSuffix(this string str, string suffix, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (!str.EndsWith(suffix, comparisonType)) return str;

        return str[^0..suffix.Length];
    }
}