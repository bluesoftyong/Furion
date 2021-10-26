// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Handles step values (i.e. */5, 2/7)
/// <remarks>
/// For example, */5 in the minutes field indicates every 5 minutes
/// </remarks>
/// </summary>
internal sealed class StepFilter : ICronFilter, ITimeFilter
{
    public CrontabFieldKind Kind { get; }

    public int Start { get; }

    public int Steps { get; }

    private int? FirstCache { get; set; }

    /// <summary>
    /// Returns a list of specific filters that represents this step filter
    /// </summary>
    public IEnumerable<SpecificFilter> SpecificFilters { get; }

    /// <summary>
    /// Constructs a new RangeFilter instance
    /// </summary>
    /// <param name="start">The start of the range</param>
    /// <param name="steps">The steps in the range</param>
    /// <param name="kind">The crontab field kind to associate with this filter</param>
    public StepFilter(int start, int steps, CrontabFieldKind kind)
    {
        var minValue = Constants.MinimumDateTimeValues[kind];
        var maxValue = Constants.MaximumDateTimeValues[kind];

        if (steps <= 0 || steps > maxValue)
            throw new TimeCrontabException(string.Format("Steps = {0} is out of bounds for <{1}> field", steps, Enum.GetName(typeof(CrontabFieldKind), kind)));

        Start = start;
        Steps = steps;
        Kind = kind;

        // We don't want our loop to necessarily start at the start point,
        // since values like 0/3 are valid for the StepFilter, but a start of
        // 0 may not be valid for the SpecificFilter instances
        var loopStart = Math.Max(start, minValue);

        var filters = new List<SpecificFilter>();
        for (var evalValue = loopStart; evalValue <= maxValue; evalValue++)
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
        return (evalValue - Start) % Steps == 0;
    }

    public int? Next(int value)
    {
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
            throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types");

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
            throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types");

        var max = Constants.MaximumDateTimeValues[Kind];

        var newValue = 0;
        while (newValue < max && !IsMatch(newValue))
            newValue++;

        if (newValue > max)
            throw new TimeCrontabException(string.Format("Next value for {0} on field {1} could not be found!",
                this.ToString(),
                Enum.GetName(typeof(CrontabFieldKind), Kind))
            );

        FirstCache = newValue;
        return newValue;
    }

    public override string ToString()
    {
        return string.Format("{0}/{1}", Start == 0 ? "*" : Start.ToString(), Steps);
    }
}