// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Furion is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Furion/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Furion.TimeCrontab;

/// <summary>
/// Cron 表达式解析类
/// </summary>
public partial class Crontab
{
    /// <summary>
    /// 解析 Cron 表达式并生成每个字段解析器字典集合
    /// </summary>
    /// <param name="expression">Cron 表达式</param>
    /// <param name="format">Cron 表达式格式化类型</param>
    /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    private static Dictionary<CrontabFieldKind, List<ICronParser>> ParseToDictionary(string expression, CronStringFormat format)
    {
        // Cron 表达式不能为null，空，纯空白
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new TimeCrontabException("The provided cron string is null, empty or contains only whitespace.");
        }

        // 根据空格切割 Cron 表达式每个字段域
        var instructions = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // 验证当前指定 Cron 表达式格式化类型数量
        var expectedCount = Constants.ExpectedFieldCounts[format];

        // 超出指定长度
        if (instructions.Length > expectedCount)
        {
            throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too many parameters.", expression));
        }
        // 小于指定长度
        if (instructions.Length < expectedCount)
        {
            throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too few parameters.", expression));
        }

        var defaultFieldOffset = 0;
        var fieldParsers = new Dictionary<CrontabFieldKind, List<ICronParser>>();

        // 解析带秒字段解析器
        if (format == CronStringFormat.WithSeconds || format == CronStringFormat.WithSecondsAndYears)
        {
            fieldParsers.Add(CrontabFieldKind.Second, ParseField(instructions[0], CrontabFieldKind.Second));
            defaultFieldOffset = 1;
        }

        // 必备 Cron 字段解析
        fieldParsers.Add(CrontabFieldKind.Minute, ParseField(instructions[defaultFieldOffset + 0], CrontabFieldKind.Minute));
        fieldParsers.Add(CrontabFieldKind.Hour, ParseField(instructions[defaultFieldOffset + 1], CrontabFieldKind.Hour));
        fieldParsers.Add(CrontabFieldKind.Day, ParseField(instructions[defaultFieldOffset + 2], CrontabFieldKind.Day));
        fieldParsers.Add(CrontabFieldKind.Month, ParseField(instructions[defaultFieldOffset + 3], CrontabFieldKind.Month));
        fieldParsers.Add(CrontabFieldKind.DayOfWeek, ParseField(instructions[defaultFieldOffset + 4], CrontabFieldKind.DayOfWeek));

        // 解析带年字段解析器
        if (format == CronStringFormat.WithYears || format == CronStringFormat.WithSecondsAndYears)
        {
            fieldParsers.Add(CrontabFieldKind.Year, ParseField(instructions[defaultFieldOffset + 5], CrontabFieldKind.Year));
        }

        // 检查非法解析器（2月份没有30号和31号的情况）
        CheckForIllegalParsers(fieldParsers);

        return fieldParsers;
    }

    /// <summary>
    /// 解析单个 Cron 字段值并转换成解析器
    /// </summary>
    /// <param name="field">Cron 字段值</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <returns><see cref="LinkedList{T}"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    private static List<ICronParser> ParseField(string field, CrontabFieldKind kind)
    {
        try
        {
            // 字段值中存在多个子值，如 1,2,3...，所以通过 , 切割
            return field.Split(',').Select(parser => ParseParser(parser, kind)).ToList();
        }
        catch (Exception ex)
        {
            throw new TimeCrontabException(
                string.Format("There was an error parsing '{0}' for the {1} field.", field, Enum.GetName(typeof(CrontabFieldKind), kind))
                , ex);
        }
    }

    /// <summary>
    /// 解析 Cron 字段值并转换成 <see cref="ICronParser"/> 解析器
    /// </summary>
    /// <param name="parser">Cron 字段值中子项值</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <returns></returns>
    /// <exception cref="TimeCrontabException"></exception>
    private static ICronParser ParseParser(string parser, CrontabFieldKind kind)
    {
        var newParser = parser.ToUpper();

        try
        {
            // 解析 * 开头的值
            if (newParser.StartsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                newParser = newParser[1..];

                // 判断是否带有 / 字符，如果有，转换成 StepParser 解析器
                if (newParser.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    // 获取步长
                    newParser = newParser[1..];
                    var steps = GetValue(ref newParser, kind);

                    return new StepParser(0, steps, kind);
                }

                return new AnyParser(kind);
            }

            // 解析 L 和 LW 值
            if (newParser.StartsWith("L") && kind == CrontabFieldKind.Day)
            {
                newParser = newParser[1..];

                if (newParser == "W")
                {
                    return new LastWeekdayOfMonthParser(kind);
                }
                else
                {
                    return new LastDayOfMonthParser(kind);
                }
            }

            // 解析 ? 值
            if (newParser == "?")
            {
                return new BlankDayOfMonthOrWeekParser(kind);
            }

            // 解析数字或字母开头值，如 2, 1/3, SUN，3W，3L，2#4，SUNL，JAN 等
            var firstValue = GetValue(ref newParser, kind);

            // 判断是否是具体值（不含字母及符号），如2，3，10
            if (string.IsNullOrEmpty(newParser))
            {
                if (kind == CrontabFieldKind.Year)
                {
                    return new SpecificYearParser(firstValue, kind);
                }
                else
                {
                    return new SpecificParser(firstValue, kind);
                }
            }

            // 如果存在符号或字符，如 - / # L W 值
            switch (newParser[0])
            {
                // 解析存在 / 符号
                case '/':
                    {
                        newParser = newParser[1..];
                        var secondValue = GetValue(ref newParser, kind);

                        return new StepParser(firstValue, secondValue, kind);
                    }
                // 解析存在 - 符号
                case '-':
                    {
                        newParser = newParser[1..];
                        var secondValue = GetValue(ref newParser, kind);
                        int? steps = null;

                        if (newParser.StartsWith("/"))
                        {
                            newParser = newParser[1..];
                            steps = GetValue(ref newParser, kind);
                        }

                        return new RangeParser(firstValue, secondValue, steps, kind);
                    }
                // 解析存在 # 符号
                case '#':
                    {
                        newParser = newParser[1..];
                        var secondValue = GetValue(ref newParser, kind);

                        if (!string.IsNullOrEmpty(newParser))
                        {
                            throw new TimeCrontabException(string.Format("Invalid parser '{0}.'", parser));
                        }

                        return new SpecificDayOfWeekInMonthParser(firstValue, secondValue, kind);
                    }
                // 解析存在 L 或 W 符号
                default:
                    if (newParser == "L" && kind == CrontabFieldKind.DayOfWeek)
                    {
                        return new LastDayOfWeekInMonthParser(firstValue, kind);
                    }
                    else if (newParser == "W" && kind == CrontabFieldKind.Day)
                    {
                        return new NearestWeekdayParser(firstValue, kind);
                    }
                    break;
            }

            throw new TimeCrontabException(string.Format("Invalid parser '{0}'.", parser));
        }
        catch (Exception ex)
        {
            throw new TimeCrontabException(string.Format("Invalid parser '{0}'. See inner exception for details.", parser), ex);
        }
    }

    /// <summary>
    /// 拨针式解析 Cron 字段值
    /// </summary>
    /// <param name="parser">Cron 字段值中子项值</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="TimeCrontabException"></exception>
    private static int GetValue(ref string parser, CrontabFieldKind kind)
    {
        // 空检查
        if (string.IsNullOrEmpty(parser))
        {
            throw new TimeCrontabException("Expected number, but parser was empty.");
        }

        int i;
        var isDigit = char.IsDigit(parser[0]);
        var isLetter = char.IsLetter(parser[0]);

        // 拨针式检查解析器每一个字符
        for (i = 0; i < parser.Length; i++)
        {
            // 如果存在不连贯数字或字母则跳出循环
            if ((isDigit && !char.IsDigit(parser[i])) || (isLetter && !char.IsLetter(parser[i])))
            {
                break;
            }
        }

        var maximum = Constants.MaximumDateTimeValues[kind];

        // 取不连贯之前的字符串
        var valueToParse = parser[..i];

        // 处理数字开头值
        if (int.TryParse(valueToParse, out var value))
        {
            // 存储下一轮拨针字符串
            parser = parser[i..];

            var returnValue = value;

            // 验证值范围
            if (returnValue > maximum)
            {
                throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
            }

            return returnValue;
        }
        // 处理字母开头值，如 SUN，SUNDAY，JAN等
        else
        {
            List<KeyValuePair<string, int>>? replaceVal = null;

            // 获取所有匹配的星期或月份单词
            if (kind == CrontabFieldKind.DayOfWeek)
            {
                replaceVal = Constants.Days.Where(x => valueToParse.StartsWith(x.Key)).ToList();
            }
            else if (kind == CrontabFieldKind.Month)
            {
                replaceVal = Constants.Months.Where(x => valueToParse.StartsWith(x.Key)).ToList();
            }

            // 判断是否完全唯一匹配
            if (replaceVal != null && replaceVal.Count == 1)
            {
                // 解析类似 SUNL 或 SUNDAYL 合法字符串
                var missingParser = "";
                if (parser.Length == i
                    && parser.EndsWith("L")
                    && kind == CrontabFieldKind.DayOfWeek)
                {
                    missingParser = "L";
                }

                parser = parser[i..] + missingParser;
                var returnValue = replaceVal.First().Value;

                // 验证值范围
                if (returnValue > maximum)
                {
                    throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
                }

                return returnValue;
            }
        }

        throw new TimeCrontabException("Parser does not contain expected number.");
    }

    /// <summary>
    /// 检查非法解析器
    /// </summary>
    /// <remarks>检查2月份没有30号和31号的情况</remarks>
    /// <param name="parsers">解析器字典集合</param>
    /// <exception cref="TimeCrontabException"></exception>
    private static void CheckForIllegalParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers)
    {
        var monthSingle = GetSpecificParsers(parsers, CrontabFieldKind.Month);
        var daySingle = GetSpecificParsers(parsers, CrontabFieldKind.Day);

        if (monthSingle.Any() && monthSingle.All(x => x.SpecificValue == 2))
        {
            if (daySingle.Any() && daySingle.All(x => (x.SpecificValue == 30) || (x.SpecificValue == 31)))
            {
                throw new TimeCrontabException("The February 30 and 31 don't exist.");
            }
        }
    }

    /// <summary>
    /// 获取所有具体值解析器
    /// </summary>
    /// <param name="parsers">解析器字典集合</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <returns><see cref="List{T}"/></returns>
    private static List<SpecificParser> GetSpecificParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers, CrontabFieldKind kind)
    {
        return parsers[kind].Where(x => x.GetType() == typeof(SpecificParser)).Cast<SpecificParser>()
            .Union(
            parsers[kind].Where(x => x.GetType() == typeof(RangeParser)).SelectMany(x => ((RangeParser)x).SpecificParsers)
            ).Union(
                parsers[kind].Where(x => x.GetType() == typeof(StepParser)).SelectMany(x => ((StepParser)x).SpecificParsers)
            ).ToList();
    }

    /// <summary>
    /// 获取特定时间范围内下一个符合的发生时间
    /// </summary>
    /// <param name="baseTime">起始计算时间</param>
    /// <param name="endTime">终止计算时间</param>
    /// <returns><see cref="DateTime"/></returns>
    private DateTime InternalGetNextOccurence(DateTime baseTime, DateTime endTime)
    {
        var newValue = baseTime;
        var overflow = true;

        // 判断是否支持秒或年格式化
        var isSecondFormat = Format == CronStringFormat.WithSeconds || Format == CronStringFormat.WithSecondsAndYears;
        var isYearFormat = Format == CronStringFormat.WithYears || Format == CronStringFormat.WithSecondsAndYears;

        // 删除时间中毫秒，不需要参与计算
        newValue = newValue.AddMilliseconds(-newValue.Millisecond);

        // 如果不启用秒支持，则删除时间中秒
        if (!isSecondFormat)
        {
            newValue = newValue.AddSeconds(-newValue.Second);
        }

        // 获取分钟和小时所有时间解析器
        var minuteParsers = Parsers[CrontabFieldKind.Minute].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
        var hourParsers = Parsers[CrontabFieldKind.Hour].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();

        // 获取秒、分、时起始值
        var firstSecondValue = newValue.Second;
        var firstMinuteValue = minuteParsers.Select(x => x.First()).Min();
        var firstHourValue = hourParsers.Select(x => x.First()).Min();

        var newSeconds = newValue.Second;

        // 处理带秒格式
        if (isSecondFormat)
        {
            // 获取最小秒取值
            var secondParsers = Parsers[CrontabFieldKind.Second].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
            firstSecondValue = secondParsers.Select(x => x.First()).Min();

            // 获取下一个发生时间，此时只有秒向前拨动
            newSeconds = Increment(secondParsers, newValue.Second, firstSecondValue, out overflow);
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);

            // 如果秒没有到终止值（59）且不匹配，重新设置起始时间
            if (!overflow && !IsMatch(newValue))
            {
                newSeconds = firstSecondValue;
                newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);
                overflow = true;
            }

            // 只有下一个秒值没有超出且完全匹配，那么直接返回
            if (!overflow)
            {
                return MinDate(newValue, endTime);
            }
        }

        // 获取下一个发生时间，此时只有分钟向前拨动
        var newMinutes = Increment(minuteParsers, newValue.Minute + (overflow ? 0 : -1), firstMinuteValue, out overflow);
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, overflow ? firstSecondValue : newSeconds);

        // 如果分钟没有到终止值（59）且不匹配，重新设置起始时间
        if (!overflow && !IsMatch(newValue))
        {
            newSeconds = firstSecondValue;
            newMinutes = firstMinuteValue;
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, firstSecondValue);
            overflow = true;
        }

        // 只有下一个分钟值没有超出且完全匹配，那么直接返回
        if (!overflow)
        {
            return MinDate(newValue, endTime);
        }

        // // 获取下一个发生时间，此时只有小时向前拨动
        var newHours = Increment(hourParsers, newValue.Hour + (overflow ? 0 : -1), firstHourValue, out overflow);
        newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newHours,
            overflow ? firstMinuteValue : newMinutes,
            overflow ? firstSecondValue : newSeconds);

        // 如果小时没有到终止值（23）且不匹配，重新设置起始时间
        if (!overflow && !IsMatch(newValue))
        {
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, firstHourValue, firstMinuteValue, firstSecondValue);
            overflow = true;
        }

        // 只有下一个小时值没有超出且完全匹配，那么直接返回
        if (!overflow)
        {
            return MinDate(newValue, endTime);
        }

        // 处理下一个年时间
        List<ITimeParser>? yearParsers = null;
        if (isYearFormat)
        {
            yearParsers = Parsers[CrontabFieldKind.Year].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
        }

        try
        {
            newValue = newValue.AddDays(1);
        }
        catch
        {
            return endTime;
        }

        // 由于计算天比较复杂，所以需要集合天，月，周，年一起参与计算
        while (!(IsMatch(newValue, CrontabFieldKind.Day)
            && IsMatch(newValue, CrontabFieldKind.DayOfWeek)
            && IsMatch(newValue, CrontabFieldKind.Month)
            && (!isYearFormat || IsMatch(newValue, CrontabFieldKind.Year))))
        {
            if (newValue >= endTime)
            {
                return MinDate(newValue, endTime);
            }

            if (isYearFormat && yearParsers!.Select(x => x.Next(newValue.Year - 1)).All(x => x == null))
            {
                return endTime;
            }

            try
            {
                newValue = newValue.AddDays(1);
            }
            catch
            {
                return endTime;
            }
        }

        return MinDate(newValue, endTime);
    }

    /// <summary>
    /// 获取当前时间解析器下一个发生值
    /// </summary>
    /// <param name="parsers">解析器</param>
    /// <param name="value">当前值</param>
    /// <param name="defaultValue">默认值</param>
    /// <param name="overflow">控制秒、分钟、小时到达59秒/分和23小时开关</param>
    /// <returns><see cref="int"/></returns>
    private static int Increment(IEnumerable<ITimeParser> parsers, int value, int defaultValue, out bool overflow)
    {
        var nextValue = parsers.Select(x => x.Next(value))
            .Where(x => x > value)
            .Min()
            ?? defaultValue;

        // 如果此时秒或分钟或23到达最大值，则应该返回起始值
        overflow = nextValue <= value;

        return nextValue;
    }

    /// <summary>
    /// 获取下一个发生时间
    /// </summary>
    /// <remarks>如果发生时间大于终止时间，则返回终止时间，否则返回发生时间</remarks>
    /// <param name="newTime">下一个发生时间</param>
    /// <param name="endTime">终止时间</param>
    /// <returns><see cref="DateTime"/></returns>
    private static DateTime MinDate(DateTime newTime, DateTime endTime)
    {
        return newTime >= endTime ? endTime : newTime;
    }

    /// <summary>
    /// 判断解析器是否全部匹配
    /// </summary>
    /// <param name="value">计算时间</param>
    /// <returns><see cref="bool"/></returns>
    private bool IsMatch(DateTime value)
    {
        return Parsers.All(fieldKind =>
            fieldKind.Value.Any(parser => parser.IsMatch(value))
        );
    }

    /// <summary>
    /// 判断解析器是否有匹配的值
    /// </summary>
    /// <param name="value">计算时间</param>
    /// <param name="kind">Cron 字段种类</param>
    /// <returns><see cref="bool"/></returns>
    private bool IsMatch(DateTime value, CrontabFieldKind kind)
    {
        return Parsers.Where(x => x.Key == kind)
            .SelectMany(x => x.Value)
            .Any(parser => parser.IsMatch(value));
    }

    /// <summary>
    /// 将 Cron 字段解析器转换成字符串
    /// </summary>
    /// <param name="paramList">Cron 字段字符串集合</param>
    /// <param name="kind">Cron 字段种类</param>
    private void JoinParsers(List<string> paramList, CrontabFieldKind kind)
    {
        paramList.Add(
            string.Join(",", Parsers
                .Where(x => x.Key == kind)
                .SelectMany(x => x.Value.Select(y => y.ToString())).ToArray()
            )
        );
    }
}