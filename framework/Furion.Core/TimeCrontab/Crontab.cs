// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

public sealed partial class Crontab
{
    private Dictionary<CrontabFieldKind, List<ICronParser>> Parsers { get; set; }

    // In the event a developer creates their own instance
    private Crontab()
    {
        Parsers = new Dictionary<CrontabFieldKind, List<ICronParser>>();
        Format = CronStringFormat.Default;
    }

    public CronStringFormat Format { get; set; }

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Crontab Parse(string expression, CronStringFormat format = CronStringFormat.Default)
    {
        return new Crontab
        {
            Format = format,
            Parsers = ParseToDictionary(expression, format)
        };
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Crontab? TryParse(string expression, CronStringFormat format = CronStringFormat.Default)
    {
        try
        {
            return Parse(expression, format);
        }
        catch (Exception)
        {
            return null;
        }
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
    /// 转换 String 输出
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var paramList = new List<string>();

        if (Format == CronStringFormat.WithSeconds || Format == CronStringFormat.WithSecondsAndYears)
            JoinParsers(paramList, CrontabFieldKind.Second);

        JoinParsers(paramList, CrontabFieldKind.Minute);
        JoinParsers(paramList, CrontabFieldKind.Hour);
        JoinParsers(paramList, CrontabFieldKind.Day);
        JoinParsers(paramList, CrontabFieldKind.Month);
        JoinParsers(paramList, CrontabFieldKind.DayOfWeek);

        if (Format == CronStringFormat.WithYears || Format == CronStringFormat.WithSecondsAndYears)
            JoinParsers(paramList, CrontabFieldKind.Year);

        return string.Join(" ", paramList.ToArray());
    }
}