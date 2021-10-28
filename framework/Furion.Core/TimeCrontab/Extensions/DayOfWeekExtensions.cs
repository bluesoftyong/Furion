// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// DayOfWeek 拓展类
/// </summary>
internal static class DayOfWeekExtensions
{
    /// <summary>
    /// 将 DayOfWeek 枚举转换成 Cron 周字段支持的数字
    /// </summary>
    /// <param name="dayOfWeek"><see cref="DayOfWeek"/> 枚举</param>
    /// <returns><see cref="int"/> 0 代表 星期日，以此类推</returns>
    internal static int ToCronDayOfWeek(this DayOfWeek dayOfWeek)
    {
        return Constants.CronDays[dayOfWeek];
    }

    /// <summary>
    /// 将 Cron 字段周数字转换成 <see cref="DayOfWeek"/> 枚举
    /// </summary>
    /// <param name="dayOfWeek">0-6 数字</param>
    /// <returns><see cref="DayOfWeek"/> 数值</returns>
    internal static DayOfWeek ToDayOfWeek(this int dayOfWeek)
    {
        return Constants.CronDays.First(x => x.Value == dayOfWeek).Key;
    }

    /// <summary>
    /// 查找特定年月中最后一个星期几是哪一天
    /// </summary>
    /// <param name="dayOfWeek"><see cref="DayOfWeek"/> 枚举</param>
    /// <param name="year">年</param>
    /// <param name="month">月</param>
    /// <returns><see cref="int"/> 返回月中对应的天数</returns>
    internal static int LastDayOfMonth(this DayOfWeek dayOfWeek, int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var date = new DateTime(year, month, daysInMonth);

        // 从月底天数进行递归查找
        while (date.DayOfWeek != dayOfWeek)
        {
            date = date.AddDays(-1);
        }

        return date.Day;
    }
}