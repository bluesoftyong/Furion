// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Handles filtering for the last specified day of the week in the month
/// </summary>
internal sealed class LastDayOfWeekInMonthFilter : ICronFilter
{
    public CrontabFieldKind Kind { get; }

    public int DayOfWeek { get; }

    private DayOfWeek DateTimeDayOfWeek { get; }

    /// <summary>
    /// Constructs a new instance of LastDayOfWeekInMonthFilter
    /// </summary>
    /// <param name="dayOfWeek">The cron day of the week (0 = Sunday...7 = Saturday)</param>
    /// <param name="kind">The crontab field kind to associate with this filter</param>
    public LastDayOfWeekInMonthFilter(int dayOfWeek, CrontabFieldKind kind)
    {
        if (kind != CrontabFieldKind.DayOfWeek)
            throw new CrontabException(string.Format("<{0}L> can only be used in the Day of Week field.", dayOfWeek));

        DayOfWeek = dayOfWeek;
        DateTimeDayOfWeek = dayOfWeek.ToDayOfWeek();
        Kind = kind;
    }

    /// <summary>
    /// Checks if the value is accepted by the filter
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value matches the condition, False if it does not match.</returns>
    public bool IsMatch(DateTime value)
    {
        return value.Day == DateTimeDayOfWeek.LastDayOfMonth(value.Year, value.Month);
    }

    public override string ToString()
    {
        return string.Format("{0}L", DayOfWeek);
    }
}