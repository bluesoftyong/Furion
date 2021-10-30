// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Cron 数字 字符解析器
/// </summary>
/// <remarks>
/// <para>表示具体值，如 1,2,3,4... 支持所有 Cron 字段种类</para>
/// </remarks>
internal class SpecificParser : ICronParser, ITimeParser
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="specificValue">具体值</param>
    /// <param name="kind">Cron 字段种类</param>
    public SpecificParser(int specificValue, CrontabFieldKind kind)
    {
        SpecificValue = specificValue;
        Kind = kind;

        // 验证值是否有效
        ValidateBounds(specificValue);
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 具体值
    /// </summary>
    public int SpecificValue { get; private set; }

    /// <summary>
    /// 是否匹配指定时间
    /// </summary>
    /// <param name="datetime">指定时间</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsMatch(DateTime datetime)
    {
        // 获取不同 Cron 字段种类对应时间值
        var evalValue = Kind switch
        {
            CrontabFieldKind.Second => datetime.Second,
            CrontabFieldKind.Minute => datetime.Minute,
            CrontabFieldKind.Hour => datetime.Hour,
            CrontabFieldKind.Day => datetime.Day,
            CrontabFieldKind.Month => datetime.Month,
            CrontabFieldKind.DayOfWeek => datetime.DayOfWeek.ToCronDayOfWeek(),
            CrontabFieldKind.Year => datetime.Year,
            _ => throw new ArgumentOutOfRangeException(nameof(datetime), Kind, null),
        };

        // 判断是否等于具体值
        return evalValue == SpecificValue;
    }

    /// <summary>
    /// 计算当前 Cron 字段种类（时间）下一个符合值
    /// </summary>
    /// <remarks>由于是具体值，所以总是返回该值</remarks>
    /// <param name="currentValue">当前值</param>
    /// <returns><see cref="int"/></returns>
    public virtual int? Next(int currentValue)
    {
        return SpecificValue;
    }

    /// <summary>
    /// 获取当前 Cron 字段种类（时间）起始值
    /// </summary>
    /// <remarks>由于是具体值，所以总是返回该值</remarks>
    /// <returns><see cref="int"/></returns>
    public int First()
    {
        return SpecificValue;
    }

    /// <summary>
    /// 重写 <see cref="ToString"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return SpecificValue.ToString();
    }

    /// <summary>
    /// 验证具体值在当前 Cron 字段种类取值范围内是否有效
    /// </summary>
    /// <param name="value">具体值</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ValidateBounds(int value)
    {
        // 获取最小值和最大值
        var minimum = Constants.MinimumDateTimeValues[Kind];
        var maximum = Constants.MaximumDateTimeValues[Kind];

        // 验证
        if (value < minimum || value > maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} should be between {minimum} and {maximum} (was {SpecificValue}).");
        }

        // 支持星期日可以同时用 0 或 7 表示
        if (Kind == CrontabFieldKind.DayOfWeek)
        {
            SpecificValue %= 7;
        }
    }
}