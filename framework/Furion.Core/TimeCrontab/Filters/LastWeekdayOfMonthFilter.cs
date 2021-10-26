// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Handles filtering for the last weekday of a month
/// </summary>
internal sealed class LastWeekdayOfMonthFilter : ICronFilter
{
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// Constructs a new RangeFilter instance
    /// </summary>
    /// <param name="kind">The crontab field kind to associate with this filter</param>
    public LastWeekdayOfMonthFilter(CrontabFieldKind kind)
    {
        if (kind != CrontabFieldKind.Day)
            throw new TimeCrontabException("<LW> can only be used in the Day field.");

        Kind = kind;
    }

    /// <summary>
    /// Checks if the value is accepted by the filter
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value matches the condition, False if it does not match.</returns>
    public bool IsMatch(DateTime value)
    {
        var specificValue = DateTime.DaysInMonth(value.Year, value.Month);
        var specificDay = new DateTime(value.Year, value.Month, specificValue);

        DateTime closestWeekday;

        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (specificDay.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                // If the specified day is Saturday, back up to Friday
                closestWeekday = specificDay.AddDays(-1);

                // If Friday is in the previous month, then move forward to the following Monday
                if (closestWeekday.Month != specificDay.Month)
                    closestWeekday = specificDay.AddDays(2);

                break;

            case DayOfWeek.Sunday:
                // If the specified day is Sunday, move forward to Monday
                closestWeekday = specificDay.AddDays(1);

                // If Monday is in the next month, then move backward to the previous Friday
                if (closestWeekday.Month != specificDay.Month)
                    closestWeekday = specificDay.AddDays(-2);

                break;

            default:
                // The specified day happens to be a weekday, so use it
                closestWeekday = specificDay;
                break;
        }

        return value.Day == closestWeekday.Day;
    }

    public override string ToString()
    {
        return "LW";
    }
}