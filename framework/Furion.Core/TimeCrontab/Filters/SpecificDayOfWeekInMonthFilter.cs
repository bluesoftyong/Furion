// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// 处理 Cron 字段 # 字符
/// </summary>
/// <remarks>
/// <para>表示每个域第几个星期几，如 4#2，仅支持 <see cref="CrontabFieldKind.DayOfWeek"/> 字段种类</para>
/// </remarks>
internal sealed class SpecificDayOfWeekInMonthFilter : ICronFilter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dayOfWeek">星期，0=星期天，7=星期六</param>
    /// <param name="weekNumber">每个月中第几个星期，一个月最多不会超过四个星期</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public SpecificDayOfWeekInMonthFilter(int dayOfWeek, int weekNumber, CrontabFieldKind kind)
    {
        // 验证星期数有效值
        if (weekNumber <= 0 || weekNumber > 5)
        {
            throw new TimeCrontabException(string.Format("Week number = {0} is out of bounds.", weekNumber));
        }

        // # 符号只能出现在 DayOfWeek 星期域
        if (kind != CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException(string.Format("<{0}#{1}> can only be used in the Day of Week field.", dayOfWeek, weekNumber));
        }

        DayOfWeek = dayOfWeek;
        DateTimeDayOfWeek = dayOfWeek.ToDayOfWeek();
        WeekNumber = weekNumber;
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
    /// 第几个周
    /// </summary>
    public int WeekNumber { get; }

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
        // 记录循环中当前日期，默认从 1 号开始
        var currentDay = new DateTime(datetime.Year, datetime.Month, 1);

        var weekCount = 0;

        // 限制当前循环仅在本月
        while (currentDay.Month == datetime.Month)
        {
            // 首先确认星期是否相等，如果相等
            if (currentDay.DayOfWeek == DateTimeDayOfWeek)
            {
                weekCount++;

                // 判断周在月中第几个是否相等，如果相等，退出循环
                if (weekCount == WeekNumber)
                {
                    break;
                }

                // 否则当前时间加 7天，继续循环
                currentDay = currentDay.AddDays(7);
            }
            // 如果星期不相等，则追加 1 天继续判断
            else
            {
                currentDay = currentDay.AddDays(1);
            }
        }

        // 处理跨月份的星期边界值
        if (currentDay.Month != datetime.Month)
        {
            return false;
        }

        return datetime.Day == currentDay.Day;
    }

    /// <summary>
    /// 重写 <see cref="ToString"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return string.Format("{0}#{1}", DayOfWeek, WeekNumber);
    }
}