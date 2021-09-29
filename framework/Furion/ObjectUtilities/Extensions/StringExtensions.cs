// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Extensions.ObjectUtilities;

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
    /// <returns>string</returns>
    internal static string DetachSuffix(this string? str, string suffix, StringComparison comparisonType = StringComparison.Ordinal)
    {
        // 空检查
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        // 判断字符串是否以制定后缀结尾
        if (!str.EndsWith(suffix, comparisonType))
        {
            return str;
        }

        // 切割字符串
        return str[0..^suffix.Length];
    }

    /// <summary>
    /// 转换字符串首字母大写
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>string</returns>
    internal static string ToTitleCase(this string? str)
    {
        // 空检查
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        // 输出首字母大写字符串
        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
    }
}