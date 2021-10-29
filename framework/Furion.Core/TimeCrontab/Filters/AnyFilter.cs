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
/// <remarks>
/// <para>表示任意值，支持所有 Cron 字段种类</para>
/// </remarks>
internal sealed class AnyFilter : ICronFilter, ITimeFilter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="kind">Cron 字段种类</param>
    public AnyFilter(CrontabFieldKind kind)
    {
        Kind = kind;
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 是否匹配指定时间
    /// </summary>
    /// <param name="datetime">指定时间</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsMatch(DateTime datetime)
    {
        return true;
    }

    /// <summary>
    /// 计算当前 Cron 字段种类下一个符合值
    /// </summary>
    /// <remarks>仅支持 Cron 字段种类为时、分、秒的种类</remarks>
    /// <param name="currentValue">当前值</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int? Next(int currentValue)
    {
        // 禁止当前 Cron 字段种类为日、月、周获取下一个符合值
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types.");
        }

        // 步长为 1 自增
        int? nextValue = currentValue + 1;

        // 判断下一个值是否在最大值内
        var maximum = Constants.MaximumDateTimeValues[Kind];
        if (nextValue > maximum)
        {
            nextValue = null;
        }

        return nextValue;
    }

    /// <summary>
    /// 获取当前 Cron 字段种类起始值
    /// </summary>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int First()
    {
        // 禁止当前 Cron 字段种类为日、月、周获取起始值
        if (Kind == CrontabFieldKind.Day
         || Kind == CrontabFieldKind.Month
         || Kind == CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types.");
        }

        return 0;
    }

    /// <summary>
    /// 重写 <see cref="ToString"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return "*";
    }
}