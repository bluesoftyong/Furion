// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Handles filtering for a specific value
/// </summary>
internal class SpecificFilter : ICronFilter, ITimeFilter
{
    public CrontabFieldKind Kind { get; }

    public int SpecificValue { get; private set; }

    /// <summary>
    /// Constructs a new RangeFilter instance
    /// </summary>
    /// <param name="specificValue">The specific value you wish to match</param>
    /// <param name="kind">The crontab field kind to associate with this filter</param>
    public SpecificFilter(int specificValue, CrontabFieldKind kind)
    {
        SpecificValue = specificValue;
        Kind = kind;

        ValidateBounds(specificValue);
    }

    private void ValidateBounds(int specificValue)
    {
        var minimum = Constants.MinimumDateTimeValues[Kind];
        var maximum = Constants.MaximumDateTimeValues[Kind];

        if (specificValue < minimum || specificValue > maximum)
            throw new ArgumentOutOfRangeException(nameof(specificValue), $"{nameof(specificValue)} should be between {minimum} and {maximum} (was {SpecificValue})");

        if (Kind == CrontabFieldKind.DayOfWeek)
        {
            // This allows Sunday to be represented by both 0 and 7
            SpecificValue %= 7;
        }
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
        return evalValue == SpecificValue;
    }

    public virtual int? Next(int value)
    {
        return SpecificValue;
    }

    public int First()
    {
        return SpecificValue;
    }

    public override string ToString()
    {
        return SpecificValue.ToString();
    }
}