// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
    internal static string SubSuffix(this string? str, string suffix, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        if (!str.EndsWith(suffix, comparisonType)) return str;

        return str[0..^suffix.Length];
    }

    /// <summary>
    /// 首字母大写
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns></returns>
    internal static string ToTitleCase(this string? str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}