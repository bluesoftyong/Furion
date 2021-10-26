// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// No specific value filter for day-of-week and day-of -month fields
/// <remarks>
/// http://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontrigger.html
/// https://en.wikipedia.org/wiki/Cron#CRON_expression
/// </remarks>
/// </summary>
internal sealed class BlankDayOfMonthOrWeekFilter : ICronFilter
{
    public CrontabFieldKind Kind { get; }

    public BlankDayOfMonthOrWeekFilter(CrontabFieldKind kind)
    {
        if (kind != CrontabFieldKind.DayOfWeek && kind != CrontabFieldKind.Day)
        {
            throw new TimeCrontabException("The <?> filter can only be used in the Day-of-Week or Day-of-Month fields.");
        }

        Kind = kind;
    }

    public bool IsMatch(DateTime value)
    {
        return true;
    }

    public int? Next(int value)
    {
        var max = Constants.MaximumDateTimeValues[Kind];
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
            throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types");

        var newValue = (int?)value + 1;
        if (newValue >= max) newValue = null;

        return newValue;
    }

    public int First()
    {
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
            throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types");

        return 0;
    }

    public override string ToString()
    {
        return "?";
    }
}