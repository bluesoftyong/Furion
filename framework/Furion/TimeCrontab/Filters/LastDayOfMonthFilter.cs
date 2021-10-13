// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Handles filtering for the last day of the month
/// </summary>
internal sealed class LastDayOfMonthFilter : ICronFilter
{
    public CrontabFieldKind Kind { get; }

    public LastDayOfMonthFilter(CrontabFieldKind kind)
    {
        if (kind != CrontabFieldKind.Day)
            throw new CrontabException("The <L> filter can only be used with the Day field.");

        Kind = kind;
    }

    /// <summary>
    /// Checks if the value is accepted by the filter
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value matches the condition, False if it does not match.</returns>
    public bool IsMatch(DateTime value)
    {
        return DateTime.DaysInMonth(value.Year, value.Month) == value.Day;
    }

    public override string ToString()
    {
        return "L";
    }
}