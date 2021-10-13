// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Handles filtering ranges (i.e. 1-5)
/// </summary>
internal sealed class RangeFilter : ICronFilter, ITimeFilter
{
    public CrontabFieldKind Kind { get; }

    public int Start { get; }

    public int End { get; }

    public int? Steps { get; }

    private int? FirstCache { get; set; }

    /// <summary>
    /// Returns a list of specific filters that represents this step filter.
    /// NOTE - This is only populated on construction, and should NOT be modified.
    /// </summary>
    public IEnumerable<SpecificFilter> SpecificFilters { get; }

    /// <summary>
    /// Constructs a new RangeFilter instance
    /// </summary>
    /// <param name="start">The start of the range</param>
    /// <param name="end">The end of the range</param>
    /// <param name="steps">The steps in the range</param>
    /// <param name="kind">The crontab field kind to associate with this filter</param>
    public RangeFilter(int start, int end, int? steps, CrontabFieldKind kind)
    {
        var maxValue = Constants.MaximumDateTimeValues[kind];

        if (start < 0 || start > maxValue)
            throw new CrontabException(string.Format("Start = {0} is out of bounds for <{1}> field", start, Enum.GetName(typeof(CrontabFieldKind), kind)));

        if (end < 0 || end > maxValue)
            throw new CrontabException(string.Format("End = {0} is out of bounds for <{1}> field", end, Enum.GetName(typeof(CrontabFieldKind), kind)));

        if (steps != null && (steps <= 0 || steps > maxValue))
            throw new CrontabException(string.Format("Steps = {0} is out of bounds for <{1}> field", steps, Enum.GetName(typeof(CrontabFieldKind), kind)));

        Start = start;
        End = end;
        Kind = kind;
        Steps = steps;

        var filters = new List<SpecificFilter>();
        for (var evalValue = Start; evalValue <= End; evalValue++)
            if (IsMatch(evalValue))
                filters.Add(new SpecificFilter(evalValue, Kind));

        SpecificFilters = filters;
    }

    /// <summary>
    /// Checks if the value is accepted by the filter
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value matches the condition, False if it does not match.</returns>
    public bool IsMatch(DateTime value)
    {
        var evalValue = Kind switch
        {
            CrontabFieldKind.Second => value.Second,
            CrontabFieldKind.Minute => value.Minute,
            CrontabFieldKind.Hour => value.Hour,
            CrontabFieldKind.Day => value.Day,
            CrontabFieldKind.Month => value.Month,
            CrontabFieldKind.DayOfWeek => value.DayOfWeek.ToCronDayOfWeek(),
            CrontabFieldKind.Year => value.Year,
            _ => throw new ArgumentOutOfRangeException(nameof(value), Kind, null),
        };

        return IsMatch(evalValue);
    }

    private bool IsMatch(int evalValue)
    {
        return evalValue >= Start && evalValue <= End && (!Steps.HasValue || ((evalValue - Start) % Steps) == 0);
    }

    public int? Next(int value)
    {
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
            throw new CrontabException("Cannot call Next for Day, Month or DayOfWeek types");

        var max = Constants.MaximumDateTimeValues[Kind];

        var newValue = (int?)value + 1;
        while (newValue < max && !IsMatch(newValue.Value))
            newValue++;

        if (newValue > max) newValue = null;

        return newValue;
    }

    public int First()
    {
        if (FirstCache.HasValue) return FirstCache.Value;

        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
            throw new CrontabException("Cannot call First for Day, Month or DayOfWeek types");

        var max = Constants.MaximumDateTimeValues[Kind];

        var newValue = 0;
        while (newValue < max && !IsMatch(newValue))
            newValue++;

        if (newValue > max)
            throw new CrontabException(string.Format("Next value for {0} on field {1} could not be found!",
                this.ToString(),
                Enum.GetName(typeof(CrontabFieldKind), Kind))
            );

        FirstCache = newValue;
        return newValue;
    }

    public override string ToString()
    {
        if (Steps.HasValue)
            return string.Format("{0}-{1}/{2}", Start, End, Steps);
        else
            return string.Format("{0}-{1}", Start, End);
    }
}