// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Cron 数字 字符解析器
/// </summary>
/// <remarks>
/// <para>表示具体值，如 1,2,3,4... 仅支持 <see cref="CrontabFieldKind.Year"/> 字段种类</para>
/// </remarks>
internal sealed class SpecificYearParser : SpecificParser
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="specificValue">具体值</param>
    /// <param name="kind">Cron 字段种类</param>
    public SpecificYearParser(int specificValue, CrontabFieldKind kind)
        : base(specificValue, kind)
    {
    }

    /// <summary>
    /// 计算当前 Cron 字段种类下一个符合值
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <returns><see cref="int"/></returns>
    public override int? Next(int currentValue)
    {
        // 如果当前年份小于具体值，则返回具体值，否则返回 null
        // 因为一旦指定了年份，那么就必须等到那一年才触发
        return currentValue < SpecificValue ? SpecificValue : null;
    }
}