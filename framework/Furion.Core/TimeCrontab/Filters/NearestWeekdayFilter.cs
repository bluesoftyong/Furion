// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// 处理 <see cref="CrontabFieldKind.Day"/> 字段 {0}W 字符
/// </summary>
/// <remarks>
/// <para>离指定日期最近的工作日，即最后一个非周六周末的日期，如 5W，当前仅处理 <see cref="CrontabFieldKind.Day"/> 字段种类</para>
/// </remarks>
internal sealed class NearestWeekdayFilter : ICronFilter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="specificValue">天（具体值）</param>
    /// <param name="kind"></param>
    /// <exception cref="TimeCrontabException">Cron 字段种类</exception>
    public NearestWeekdayFilter(int specificValue, CrontabFieldKind kind)
    {
        // 限制当前过滤器只能作用于 Cron 字段种类 Day 域
        if (kind != CrontabFieldKind.Day)
        {
            throw new TimeCrontabException(string.Format("<{0}W> can only be used in the Day field.", specificValue));
        }

        // 验证具体值范围
        var maximum = Constants.MaximumDateTimeValues[CrontabFieldKind.Day];
        if (specificValue <= 0 || specificValue > maximum)
        {
            throw new TimeCrontabException(string.Format("<{0}W> is out of bounds for the Day field.", specificValue));
        }

        SpecificValue = specificValue;
        Kind = kind;
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 天（具体值）
    /// </summary>
    public int SpecificValue { get; }

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

        // 如果这个月没有足够的天数则跳过（例如，二月没有与31日最接近的工作日，因为没有二月31日。）
        if (DateTime.DaysInMonth(datetime.Year, datetime.Month) < SpecificValue)
        {
            return false;
        }

        // 获取当前时间日期
        var specificDay = new DateTime(datetime.Year, datetime.Month, SpecificValue);

        DateTime closestWeekday;

        // 处理当天的不同情况
        switch (specificDay.DayOfWeek)
        {
            // 如果当天是周六，则退一天
            case DayOfWeek.Saturday:
                closestWeekday = specificDay.AddDays(-1);

                // 如果退一天不在本月，则转到下周一
                if (closestWeekday.Month != specificDay.Month)
                {
                    closestWeekday = specificDay.AddDays(2);
                }

                break;

            // 如果当天是周天，则进一天
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
        return string.Format("{0}W", SpecificValue);
    }
}