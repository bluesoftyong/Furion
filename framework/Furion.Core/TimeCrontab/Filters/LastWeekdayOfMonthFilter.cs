// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// 处理 <see cref="CrontabFieldKind.Day"/> 字段 LW 字符
/// </summary>
/// <remarks>
/// <para>月中最后一个工作日，即最后一个非周六周末的日期，如 LW，当前仅处理 <see cref="CrontabFieldKind.Day"/> 字段种类</para>
/// </remarks>
internal sealed class LastWeekdayOfMonthFilter : ICronFilter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public LastWeekdayOfMonthFilter(CrontabFieldKind kind)
    {
        // 限制当前过滤器只能作用于 Cron 字段种类 Day 域
        if (kind != CrontabFieldKind.Day)
        {
            throw new TimeCrontabException("<LW> can only be used in the Day field.");
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
        /*
         * W: 表示有效工作日(周一到周五),只能出现在 Day 域，系统将在离指定日期的最近的有效工作日触发事件。
         * 例如：在 Day 使用 5W，如果 5 日是星期六，则将在最近的工作日：星期五，即 4 日触发。
         * 如果 5 日是星期天，则在 6 日(周一)触发；如果5日在星期一到星期五中的一天，则就在 5 日触发。
         * 另外一点，W 的最近寻找不会跨过月份
         */

        // 获取当前时间所在月最后一天日期
        var specificValue = DateTime.DaysInMonth(datetime.Year, datetime.Month);
        var specificDay = new DateTime(datetime.Year, datetime.Month, specificValue);

        // 最靠近的工作日时间
        DateTime closestWeekday;

        // 处理月中最后一天的不同情况
        switch (specificDay.DayOfWeek)
        {
            // 如果最后一天是周六，则退一天
            case DayOfWeek.Saturday:
                closestWeekday = specificDay.AddDays(-1);

                //// 如果退一天不在本月，则转到下周一
                //if (closestWeekday.Month != specificDay.Month)
                //{
                //    closestWeekday = specificDay.AddDays(2);
                //}

                break;

            // 如果最后一天是周天，则进一天
            case DayOfWeek.Sunday:
                closestWeekday = specificDay.AddDays(1);

                // 如果进一天不在本月，则退到上周五
                if (closestWeekday.Month != specificDay.Month)
                {
                    closestWeekday = specificDay.AddDays(-2);
                }

                break;

            // 处理恰好是工作日情况，直接使用
            default:
                closestWeekday = specificDay;
                break;
        }

        return datetime.Day == closestWeekday.Day;
    }

    /// <summary>
    /// 重写 <see cref="ToString"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return "LW";
    }
}