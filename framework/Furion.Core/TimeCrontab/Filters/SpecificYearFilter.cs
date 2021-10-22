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
internal sealed class SpecificYearFilter : SpecificFilter
{
    /// <summary>
    /// Constructs a new RangeFilter instance
    /// </summary>
    /// <param name="specificValue">The specific value you wish to match</param>
    /// <param name="kind">The crontab field kind to associate with this filter</param>
    public SpecificYearFilter(int specificValue, CrontabFieldKind kind)
        : base(specificValue, kind)
    {
    }

    public override int? Next(int value)
    {
        if (value < SpecificValue)
            return SpecificValue;

        return null;
    }
}