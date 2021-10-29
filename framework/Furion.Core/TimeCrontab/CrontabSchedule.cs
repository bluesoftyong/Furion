// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

public sealed partial class CrontabSchedule
{
    private Dictionary<CrontabFieldKind, List<ICronFilter>> Filters { get; set; }

    // In the event a developer creates their own instance
    public CrontabSchedule()
    {
        Filters = new Dictionary<CrontabFieldKind, List<ICronFilter>>();
        Format = CronStringFormat.Default;
    }

    public CronStringFormat Format { get; set; }

    /// <summary>
    /// 转换 String 输出
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var paramList = new List<string>();

        if (Format == CronStringFormat.WithSeconds || Format == CronStringFormat.WithSecondsAndYears)
            JoinFilters(paramList, CrontabFieldKind.Second);

        JoinFilters(paramList, CrontabFieldKind.Minute);
        JoinFilters(paramList, CrontabFieldKind.Hour);
        JoinFilters(paramList, CrontabFieldKind.Day);
        JoinFilters(paramList, CrontabFieldKind.Month);
        JoinFilters(paramList, CrontabFieldKind.DayOfWeek);

        if (Format == CronStringFormat.WithYears || Format == CronStringFormat.WithSecondsAndYears)
            JoinFilters(paramList, CrontabFieldKind.Year);

        return string.Join(" ", paramList.ToArray());
    }

    /// <summary>
    /// 获取下一个执行时间，没有结束边界
    /// </summary>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public DateTime GetNextOccurrence(DateTime baseValue)
    {
        return GetNextOccurrence(baseValue, DateTime.MaxValue);
    }

    /// <summary>
    /// 获取下一个执行时间，带结束边界
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    public DateTime GetNextOccurrence(DateTime baseValue, DateTime endValue)
    {
        return InternalGetNextOccurence(baseValue, endValue);
    }

    /// <summary>
    /// 获取后面所有执行时间，带结束边界
    /// </summary>
    /// <param name="baseTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public IEnumerable<DateTime> GetNextOccurrences(DateTime baseTime, DateTime endTime)
    {
        for (var occurrence = GetNextOccurrence(baseTime, endTime);
             occurrence < endTime;
             occurrence = GetNextOccurrence(occurrence, endTime))
        {
            yield return occurrence;
        }
    }

    /// <summary>
    /// 获取下一个值
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <param name="overflow"></param>
    /// <returns></returns>
    private static int Increment(IEnumerable<ITimeFilter> filters, int value, int defaultValue, out bool overflow)
    {
        var nextValue = filters.Select(x => x.Next(value)).Where(x => x > value).Min() ?? defaultValue;
        overflow = nextValue <= value;
        return nextValue;
    }

    /// <summary>
    /// 最小日期
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    private static DateTime MinDate(DateTime newValue, DateTime endValue)
    {
        return newValue >= endValue ? endValue : newValue;
    }

    /// <summary>
    /// 内部计算
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="endValue"></param>
    /// <returns></returns>
    private DateTime InternalGetNextOccurence(DateTime baseValue, DateTime endValue)
    {
        var newValue = baseValue;
        var overflow = true;

        var isSecondFormat = Format == CronStringFormat.WithSeconds || Format == CronStringFormat.WithSecondsAndYears;
        var isYearFormat = Format == CronStringFormat.WithYears || Format == CronStringFormat.WithSecondsAndYears;

        // First things first - trim off any time components we don't need
        newValue = newValue.AddMilliseconds(-newValue.Millisecond);
        if (!isSecondFormat) newValue = newValue.AddSeconds(-newValue.Second);

        var minuteFilters = Filters[CrontabFieldKind.Minute].Where(x => x is ITimeFilter).Cast<ITimeFilter>().ToList();
        var hourFilters = Filters[CrontabFieldKind.Hour].Where(x => x is ITimeFilter).Cast<ITimeFilter>().ToList();

        var firstSecondValue = newValue.Second;
        var firstMinuteValue = minuteFilters.Select(x => x.First()).Min();
        var firstHourValue = hourFilters.Select(x => x.First()).Min();

        var newSeconds = newValue.Second;
        if (isSecondFormat)
        {
            var secondFilters = Filters[CrontabFieldKind.Second].Where(x => x is ITimeFilter).Cast<ITimeFilter>().ToList();
            firstSecondValue = secondFilters.Select(x => x.First()).Min();
            newSeconds = Increment(secondFilters, newValue.Second, firstSecondValue, out overflow);
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);
            if (!overflow && !IsMatch(newValue))
            {
                newSeconds = firstSecondValue;
                newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);
                overflow = true;
            }
            if (!overflow) return MinDate(newValue, endValue);
        }

        var newMinutes = Increment(minuteFilters, newValue.Minute + (overflow ? 0 : -1), firstMinuteValue, out overflow);
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, overflow ? firstSecondValue : newSeconds);
        if (!overflow && !IsMatch(newValue))
        {
            newSeconds = firstSecondValue;
            newMinutes = firstMinuteValue;
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, firstSecondValue);
            overflow = true;
        }
        if (!overflow) return MinDate(newValue, endValue);

        var newHours = Increment(hourFilters, newValue.Hour + (overflow ? 0 : -1), firstHourValue, out overflow);
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newHours,
            overflow ? firstMinuteValue : newMinutes,
            overflow ? firstSecondValue : newSeconds);

        if (!overflow && !IsMatch(newValue))
        {
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, firstHourValue, firstMinuteValue, firstSecondValue);
            overflow = true;
        }

        if (!overflow) return MinDate(newValue, endValue);

        List<ITimeFilter>? yearFilters = null;
        if (isYearFormat) yearFilters = Filters[CrontabFieldKind.Year].Where(x => x is ITimeFilter).Cast<ITimeFilter>().ToList();

        // Sooo, this is where things get more complicated.
        // Since the filtering of days relies on what month/year you're in
        // (for weekday/nth day filters), we'll only increment the day, and
        // check all day/month/year filters.  Might be a litle slow, but we
        // won't miss any days that way.

        // Also, if we increment to the next day, we need to set the hour, minute and second
        // fields to their "first" values, since that would be the earliest they'd run.  We
        // only have to do this after the initial AddDays call.  FYI - they're already at their
        // first values if overflowHour = True.  :-)

        // This feels so dirty.  This is to catch the odd case where you specify
        // 12/31/9999 23:59:59.999 as your end date, and you don't have any matches,
        // so it reaches the max value of DateTime and throws an exception.
        try { newValue = newValue.AddDays(1); } catch { return endValue; }

        while (!(IsMatch(newValue, CrontabFieldKind.Day) && IsMatch(newValue, CrontabFieldKind.DayOfWeek) && IsMatch(newValue, CrontabFieldKind.Month) && (!isYearFormat || IsMatch(newValue, CrontabFieldKind.Year))))
        {
            if (newValue >= endValue) return MinDate(newValue, endValue);

            // In instances where the year is filtered, this will speed up the path to get to endValue
            // (without having to actually go to endValue)
            if (isYearFormat && yearFilters!.Select(x => x.Next(newValue.Year - 1)).All(x => x == null)) return endValue;

            // Ugh...have to do the try/catch again...
            try { newValue = newValue.AddDays(1); } catch { return endValue; }
        }

        return MinDate(newValue, endValue);
    }

    /// <summary>
    /// 是否匹配
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool IsMatch(DateTime value)
    {
        return Filters.All(fieldKind =>
            fieldKind.Value.Any(filter => filter.IsMatch(value))
        );
    }

    /// <summary>
    /// 是否匹配
    /// </summary>
    /// <param name="value"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    private bool IsMatch(DateTime value, CrontabFieldKind kind)
    {
        return Filters.Where(x => x.Key == kind).SelectMany(x => x.Value).Any(filter => filter.IsMatch(value));
    }

    private void JoinFilters(List<string> paramList, CrontabFieldKind kind)
    {
        paramList.Add(
            string.Join(",", Filters
                .Where(x => x.Key == kind)
                .SelectMany(x => x.Value.Select(y => y.ToString())).ToArray()
            )
        );
    }
}