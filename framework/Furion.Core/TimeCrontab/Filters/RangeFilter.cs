// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// 处理 Cron 字段 - 字符
/// </summary>
/// <remarks>
/// <para>表示特定访问，如 1-5 或 1-5/2，支持所有 Cron 字段种类</para>
/// </remarks>
internal sealed class RangeFilter : ICronFilter, ITimeFilter
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="start">起始值</param>
    /// <param name="end">终止值</param>
    /// <param name="steps">步长</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <exception cref="TimeCrontabException"></exception>
    public RangeFilter(int start, int end, int? steps, CrontabFieldKind kind)
    {
        var maximum = Constants.MaximumDateTimeValues[kind];

        // 验证起始值
        if (start < 0 || start > maximum)
        {
            throw new TimeCrontabException(string.Format("Start = {0} is out of bounds for <{1}> field", start, Enum.GetName(typeof(CrontabFieldKind), kind)));
        }

        // 验证终止值
        if (end < 0 || end > maximum)
        {
            throw new TimeCrontabException(string.Format("End = {0} is out of bounds for <{1}> field", end, Enum.GetName(typeof(CrontabFieldKind), kind)));
        }

        // 验证步长
        if (steps != null && (steps <= 0 || steps > maximum))
        {
            throw new TimeCrontabException(string.Format("Steps = {0} is out of bounds for <{1}> field", steps, Enum.GetName(typeof(CrontabFieldKind), kind)));
        }

        Start = start;
        End = end;
        Kind = kind;
        Steps = steps;

        // 循环计算当前 Cron 字段种类符合取值范围的所有值并存入 SpecificFilters 中
        var filters = new List<SpecificFilter>();
        for (var evalValue = Start; evalValue <= End; evalValue++)
        {
            if (IsMatch(evalValue))
            {
                filters.Add(new SpecificFilter(evalValue, Kind));
            }
        }

        SpecificFilters = filters;
    }

    /// <summary>
    /// Cron 字段种类
    /// </summary>
    public CrontabFieldKind Kind { get; }

    /// <summary>
    /// 起始值
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// 终止值
    /// </summary>
    public int End { get; }

    /// <summary>
    /// 步长
    /// </summary>
    public int? Steps { get; }

    /// <summary>
    /// 所有符合范围值或带步长算法的具体值过滤器
    /// </summary>
    public IEnumerable<SpecificFilter> SpecificFilters { get; }

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

        return IsMatch(evalValue);
    }

    /// <summary>
    /// 计算当前 Cron 字段种类（时间）下一个符合值
    /// </summary>
    /// <remarks>仅支持 Cron 字段种类为时、分、秒的种类</remarks>
    /// <param name="currentValue">当前值</param>
    /// <returns><see cref="int"/></returns>
    public int? Next(int currentValue)
    {
        // 禁止当前 Cron 字段种类为日、月、周获取下一个符合值
        if (Kind == CrontabFieldKind.Day
            || Kind == CrontabFieldKind.Month
            || Kind == CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types.");
        }

        var maximum = Constants.MaximumDateTimeValues[Kind];
        int? newValue = currentValue + 1;

        // 获取下一个符合的值，值必须小于最大值且符合范围值或带步长算法内
        while (newValue < maximum && !IsMatch(newValue.Value))
        {
            newValue++;
        }

        return newValue > maximum ? null : newValue;
    }

    /// <summary>
    /// 避免重复计算进而起始值
    /// </summary>
    /// <remarks>处理带步长的范围字符串</remarks>
    private int? FirstCache { get; set; }

    /// <summary>
    /// 获取当前 Cron 字段种类（时间）起始值
    /// </summary>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    public int First()
    {
        if (FirstCache.HasValue) return FirstCache.Value;

        if (Kind == CrontabFieldKind.Day
            || Kind == CrontabFieldKind.Month
            || Kind == CrontabFieldKind.DayOfWeek)
        {
            throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types.");
        }

        var maximum = Constants.MaximumDateTimeValues[Kind];
        var newValue = 0;

        // 获取首个符合的值，值必须小于最大值且符合范围值或带步长算法内
        while (newValue < maximum && !IsMatch(newValue))
        {
            newValue++;
        }

        // 验证起始值边界
        if (newValue > maximum)
            throw new TimeCrontabException(
                string.Format("Next value for {0} on field {1} could not be found!",
                ToString(),
                Enum.GetName(typeof(CrontabFieldKind), Kind))
            );

        // 缓存起始值
        FirstCache = newValue;
        return newValue;
    }

    /// <summary>
    /// 重写 <see cref="ToString"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return Steps.HasValue
                 ? string.Format("{0}-{1}/{2}", Start, End, Steps)
                 : string.Format("{0}-{1}", Start, End);
    }

    /// <summary>
    /// 判断单个值是否符合取值范围或带步长算法
    /// </summary>
    /// <param name="evalValue">值</param>
    /// <returns><see cref="bool"/></returns>
    private bool IsMatch(int evalValue)
    {
        return evalValue >= Start && evalValue <= End
            && (!Steps.HasValue || ((evalValue - Start) % Steps) == 0);
    }
}