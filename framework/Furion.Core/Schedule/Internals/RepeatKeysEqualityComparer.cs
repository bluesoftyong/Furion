// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Diagnostics.CodeAnalysis;

namespace Furion.Schedule;

/// <summary>
/// 支持重复 Key 的字典集合比较器
/// </summary>
internal sealed class StringRepeatKeysEqualityComparer : IEqualityComparer<string>
{
    /// <summary>
    /// 实现相等逻辑判断
    /// </summary>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    /// <returns><see cref="bool"/></returns>
    public bool Equals(string? x, string? y)
    {
        return x != y;
    }

    /// <summary>
    /// 获取对象 HashCode
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns><see cref="int"/></returns>
    public int GetHashCode([DisallowNull] string obj)
    {
        return obj.GetHashCode();
    }
}