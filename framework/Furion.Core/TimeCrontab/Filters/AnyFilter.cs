// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// 处理 Cron 字段 * 字符
/// </summary>
/// <remarks>表示任意时间点</remarks>
internal sealed class AnyFilter : ICronFilter, ITimeFilter
{
    /// <summary>
    /// Cron 表达式字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="kind">Cron 表达式字段种类</param>
    public AnyFilter(CrontabFieldKind kind)
    {
        Kind = kind;
    }

    /// <summary>
    /// Checks if the value is accepted by the filter
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value matches the condition, False if it does not match.</returns>
    public bool IsMatch(DateTime value)
    {
        return true;
    }

    /// <summary>
    /// 获取当前时间下一个值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int? Next(int value)
    {
        var max = Constants.MaximumDateTimeValues[Kind];
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
            throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types");

        var newValue = (int?)value + 1;
        if (newValue > max) newValue = null;

        return newValue;
    }

    /// <summary>
    /// 获取时间起始值
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int First()
    {
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
            throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types");

        return 0;
    }

    /// <summary>
    /// 转换成 Cron 字符串
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return "*";
    }
}