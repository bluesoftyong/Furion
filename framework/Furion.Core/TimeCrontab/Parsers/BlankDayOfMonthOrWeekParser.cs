// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Cron ? 字符解析器
/// </summary>
/// <remarks>
/// <para>只能用在 Day 和 DayOfWeek 两个域。它也匹配域的任意值，但实际不会。因为 Day 和 DayOfWeek 会相互影响。</para>
/// <para>例如想在每月的 20 日触发调度，不管 20 日到底是星期几，则只能使用如下写法： 13 13 15 20 * ?</para>
/// <para>其中最后一位只能用 ?，而不能使用 *，如果使用 * 表示不管星期几都会触发，实际上并不是这样。</para>
/// <para>所以 ? 实际上是起着 互斥性 作用</para>
/// <para>仅支持 <see cref="CrontabFieldKind.Day"/> 或 <see cref="CrontabFieldKind.DayOfWeek"/> 字段</para>
/// </remarks>
internal sealed class BlankDayOfMonthOrWeekParser : ICronParser
{
    /// <summary>
    ///  构造函数
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public BlankDayOfMonthOrWeekParser(CrontabFieldKind kind)
    {
        // 限制当前过滤器只能作用于 Cron 字段种类 Day 和 DayOfWeek 域
        if (kind != CrontabFieldKind.DayOfWeek && kind != CrontabFieldKind.Day)
        {
            throw new TimeCrontabException("The <?> filter can only be used in the Day-of-Week or Day-of-Month fields.");
        }

        Kind = kind;
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 是否匹配指定时间
    /// </summary>
    /// <param name="datetime">指定时间</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsMatch(DateTime datetime)
    {
        return true;
    }

    /// <summary>
    /// 计算当前 Cron 字段种类（时间）下一个符合值
    /// </summary>
    /// <remarks>仅支持 Cron 字段种类为时、分、秒的种类</remarks>
    /// <param name="currentValue">当前值</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int? Next(int currentValue)
    {
        // 禁止当前 Cron 字段种类为日、月、周获取下一个符合值
        if (Kind == CrontabFieldKind.Day
            || Kind == CrontabFieldKind.Month
            || Kind == CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types.");
        }

        // 步长为 1 自增
        int? newValue = currentValue + 1;

        // 判断下一个值是否在最大值内
        var maximum = Constants.MaximumDateTimeValues[Kind];
        return newValue >= maximum ? null : newValue;
    }

    /// <summary>
    /// 获取当前 Cron 字段种类（时间）起始值
    /// </summary>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int First()
    {
        // 禁止当前 Cron 字段种类为日、月、周获取起始值
        if (Kind == CrontabFieldKind.Day
            || Kind == CrontabFieldKind.Month
            || Kind == CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types.");
        }

        return 0;
    }

    /// <summary>
    /// 重写 <see cref="ToString"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return "?";
    }
}