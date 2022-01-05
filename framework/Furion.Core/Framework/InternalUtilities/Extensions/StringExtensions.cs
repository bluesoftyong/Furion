// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.Extensions.InternalUtilities;

/// <summary>
/// <see cref="string"/> 拓展
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// 移除字符串指定后缀
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="suffix">后缀字符串</param>
    /// <param name="comparisonType">字符串比较方式</param>
    /// <returns>移除指定后缀的文本</returns>
    internal static string DetachSuffix(this string? text
        , string suffix
        , StringComparison comparisonType = StringComparison.Ordinal)
    {
        // 空检查
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        // 判断字符串是否以制定后缀结尾
        if (!text.EndsWith(suffix, comparisonType))
        {
            return text;
        }

        // 切割字符串
        return text[0..^suffix.Length];
    }

    /// <summary>
    /// 转换字符串首字母大写
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>首字母大写文本</returns>
    internal static string ToTitleCase(this string? text)
    {
        // 空检查
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        // 输出首字母大写字符串
        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(text);
    }
}