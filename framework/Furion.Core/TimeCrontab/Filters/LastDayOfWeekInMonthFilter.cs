// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// 处理 <see cref="CrontabFieldKind.DayOfWeek"/> 字段 {0}L 字符
/// </summary>
/// <remarks>
/// <para>月中最后一个星期几，如 5L，当前仅处理 <see cref="CrontabFieldKind.DayOfWeek"/> 字段种类</para>
/// </remarks>
internal sealed class LastDayOfWeekInMonthFilter : ICronFilter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dayOfWeek">星期，0=星期天，7=星期六</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public LastDayOfWeekInMonthFilter(int dayOfWeek, CrontabFieldKind kind)
    {
        // 限制当前过滤器只能作用于 Cron 字段种类 DayOfWeek 域
        if (kind != CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException(string.Format("<{0}L> can only be used in the Day of Week field.", dayOfWeek));
        }

        DayOfWeek = dayOfWeek;
        DateTimeDayOfWeek = dayOfWeek.ToDayOfWeek();
        Kind = kind;
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 星期几
    /// </summary>
    public int DayOfWeek { get; }

    /// <summary>
    /// <see cref="DayOfWeek"/> 类型星期几
    /// </summary>
    private DayOfWeek DateTimeDayOfWeek { get; }

    /// <summary>
    /// 是否匹配指定时间
    /// </summary>
    /// <param name="datetime">指定时间</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsMatch(DateTime datetime)
    {
        return datetime.Day == DateTimeDayOfWeek.LastDayOfMonth(datetime.Year, datetime.Month);
    }

    /// <summary>
    /// 重写 <see cref="ToString"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return string.Format("{0}L", DayOfWeek);
    }
}